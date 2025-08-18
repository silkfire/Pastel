using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace SimpleTypesExtensionsGenerator;

[Generator]
public sealed class SimpleTypesExtensionsSourceGenerator : ISourceGenerator
{
    private static readonly string[] TargetTypes =
        [
            "bool",
            "byte",
            "char",
            "decimal",
            "double",
            "float",
            "int",
            "long",
            "sbyte",
            "short",
            "uint",
            "ulong",
            "ushort"
        ];

    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            IAssemblySymbol? pastelAssembly;

            if (context.Compilation.AssemblyName == "Pastel")
            {
                pastelAssembly = context.Compilation.Assembly;
            }
            else
            {
                pastelAssembly = context.Compilation.SourceModule.ReferencedAssemblySymbols
                                        .FirstOrDefault(a => a.Name == "Pastel");
            }

            if (pastelAssembly == null) return;

            var stringExtensions = FindStringExtensionMethods(pastelAssembly);

            if (!stringExtensions.Any()) return;

            var toStringMethods = FindToStringMethods(context.Compilation);
            var generatedCode = GenerateCombinedExtensions(stringExtensions, toStringMethods);

            context.AddSource("PastelExtensions.g.cs", generatedCode);
        }
        catch (Exception ex)
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "PEG001",
                    "PastelExtensionsGenerator Error",
                    $"Error in PastelExtensionsGenerator: {ex.Message}",
                    "PastelExtensionsGenerator",
                    DiagnosticSeverity.Warning,
                    true),
                Location.None);

            context.ReportDiagnostic(diagnostic);
        }
    }

    private static List<ExtensionMethodInfo> FindStringExtensionMethods(IAssemblySymbol pastelAssembly)
    {
        var extensions = new List<ExtensionMethodInfo>();

        foreach (var module in pastelAssembly.Modules)
        {
            foreach (var ns in GetAllNamespaces(module.GlobalNamespace))
            {
                foreach (var type in ns.GetTypeMembers())
                {
                    if (!type.IsStatic) continue;

                    foreach (var member in type.GetMembers())
                    {
                        if (member is IMethodSymbol { IsExtensionMethod: true, Parameters.Length: > 0 } method &&
                            method.Parameters[0].Type.SpecialType == SpecialType.System_String)
                        {
                            extensions.Add(new ExtensionMethodInfo
                            {
                                MethodName = method.Name,
                                ReturnType = method.ReturnType.ToDisplayString(),
                                Parameters = method.Parameters.Skip(1).Select(p => new ParameterInfo
                                {
                                    Name = p.Name,
                                    Type = p.Type.ToDisplayString(),
                                    HasDefaultValue = p.HasExplicitDefaultValue,
                                    DefaultValue = p.HasExplicitDefaultValue ? FormatDefaultValue(p.ExplicitDefaultValue) : null
                                }).ToList(),
                                Xml = method.GetDocumentationCommentXml(expandIncludes: true)
                            });
                        }
                    }
                }
            }
        }

        return extensions;
    }

    private static List<ToStringMethodInfo> FindToStringMethods(Compilation compilation)
    {
        var toStringMethods = new List<ToStringMethodInfo>();

        foreach (var targetType in TargetTypes)
        {
            var typeSymbol = compilation.GetSpecialType(GetSpecialType(targetType));

            foreach (var member in typeSymbol.GetMembers("ToString"))
            {
                if (member is IMethodSymbol { IsStatic: false } method)
                {
                    toStringMethods.Add(new ToStringMethodInfo
                    {
                        TargetType = targetType,
                        Parameters = method.Parameters
                                           .Select(p => new ParameterInfo
                                           {
                                               Name = p.Name,
                                               Type = p.Type.ToDisplayString(),
                                               HasDefaultValue = p.HasExplicitDefaultValue,
                                               DefaultValue = p.HasExplicitDefaultValue ? FormatDefaultValue(p.ExplicitDefaultValue) : null
                                           })
                                           .ToList(),
                        Xml = method.GetDocumentationCommentXml(expandIncludes: true)
                    });
                }
            }
        }

        return toStringMethods;
    }

    private static string GenerateCombinedExtensions(List<ExtensionMethodInfo> stringExtensions, List<ToStringMethodInfo> toStringMethods)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Drawing;");
        sb.AppendLine();
        sb.AppendLine("namespace Pastel");
        sb.AppendLine("{");
        sb.AppendLine("    public static class GeneratedExtensions");
        sb.AppendLine("    {");

        foreach (var stringExt in stringExtensions)
        {
            foreach (var toString in toStringMethods)
            {
                GenerateCombinedMethod(sb, stringExt, toString);
            }
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static void GenerateCombinedMethod(StringBuilder sb, ExtensionMethodInfo stringExt, ToStringMethodInfo toString)
    {
        var allParams = new List<ParameterInfo>();
        allParams.AddRange(stringExt.Parameters);
        allParams.AddRange(toString.Parameters);

        allParams = allParams
                    .OrderBy(p => p.HasDefaultValue)
                    .ToList();

        var paramOrder = allParams
                         .Select((p, i) => new { p.Name, Index = i })
                         .ToDictionary(x => x.Name, x => x.Index);

        var xmlDoc = new XmlDoc();
        LoadXmlDocumentation(toString.Xml, xmlDoc, toString.Parameters);
        LoadXmlDocumentation(stringExt.Xml, xmlDoc, stringExt.Parameters);
        sb.AppendLine(xmlDoc.ToString(paramOrder, "        "));

        sb.AppendLine(
            $"        public static {stringExt.ReturnType} {stringExt.MethodName}(this {toString.TargetType} value{(allParams.Any() ? ", " : "")}{string.Join(", ", allParams.Select(FormatParameter))})");
        sb.AppendLine("        {");

        var toStringCall = toString.Parameters.Any()
            ? $"value.ToString({string.Join(", ", toString.Parameters.Select(p => p.Name))})"
            : "value.ToString()";

        var pastelCall = stringExt.Parameters.Any()
            ? $".{stringExt.MethodName}({string.Join(", ", stringExt.Parameters.Select(p => p.Name))})"
            : $".{stringExt.MethodName}()";

        sb.AppendLine($"            return {toStringCall}{pastelCall};");
        sb.AppendLine("        }");
        sb.AppendLine();
    }

    private static void LoadXmlDocumentation(string? xmlStr, XmlDoc xmlDoc, List<ParameterInfo> parameters)
    {
        if (string.IsNullOrEmpty(xmlStr)) return;

        var xmDocument = XDocument.Parse(xmlStr);
        var xmlMember = xmDocument.Element("member");

        if (xmlMember != null)
        {
            var summary = xmlMember.Element("summary")?.Value.Trim();
            if (!string.IsNullOrEmpty(summary)) xmlDoc.Summary.Add(summary!);

            var returns = xmlMember.Element("returns");
            if (returns != null) xmlDoc.Returns.Add(returns.ToString());

            xmlDoc.Parameters.AddRange(xmlMember.Elements("param")
                                                .Where(p => parameters.Select(p => p.Name)
                                                                      .Contains(p.Attribute("name")?.Value))
                                                .ToList());
        }
    }

    private static string FormatParameter(ParameterInfo param)
    {
        var result = $"{param.Type} {param.Name}";

        if (param.HasDefaultValue) result = $"{result} = {param.DefaultValue}";

        return result;
    }

    private static string FormatDefaultValue(object? value)
    {
        return value switch
        {
            null => "null",
            string str => $"\"{str}\"",
            bool b => b ? "true" : "false",
            char c => $"'{c}'",
            byte by => $"{by}",
            sbyte sb => $"{sb}",
            short s => $"{s}",
            ushort us => $"{us}",
            int i => $"{i}",
            uint ui => $"{ui}u",
            long l => $"{l}L",
            ulong ul => $"{ul}UL",
            float f => $"{f}f",
            double d => $"{d}d",
            decimal m => $"{m}m",
            _ => value.ToString() ?? "null"
        };
    }

    private static SpecialType GetSpecialType(string typeName)
    {
        return typeName switch
        {
            "bool" => SpecialType.System_Boolean,
            "byte" => SpecialType.System_Byte,
            "char" => SpecialType.System_Char,
            "decimal" => SpecialType.System_Decimal,
            "double" => SpecialType.System_Double,
            "float" => SpecialType.System_Single,
            "int" => SpecialType.System_Int32,
            "long" => SpecialType.System_Int64,
            "sbyte" => SpecialType.System_SByte,
            "short" => SpecialType.System_Int16,
            "uint" => SpecialType.System_UInt32,
            "ulong" => SpecialType.System_UInt64,
            "ushort" => SpecialType.System_UInt16,
            _ => SpecialType.None
        };
    }

    private static IEnumerable<INamespaceSymbol> GetAllNamespaces(INamespaceSymbol root)
    {
        yield return root;
        foreach (var child in root.GetNamespaceMembers())
        {
            foreach (var nested in GetAllNamespaces(child))
            {
                yield return nested;
            }
        }
    }

    private sealed class ExtensionMethodInfo
    {
        public string MethodName { get; set; } = string.Empty;
        public string ReturnType { get; set; } = string.Empty;
        public List<ParameterInfo> Parameters { get; set; } = [];
        public string? Xml { get; set; }
    }

    private sealed class ToStringMethodInfo
    {
        public string TargetType { get; set; } = string.Empty;
        public List<ParameterInfo> Parameters { get; set; } = [];
        public string? Xml { get; set; }
    }

    private sealed class ParameterInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool HasDefaultValue { get; set; }
        public string? DefaultValue { get; set; }
    }

    private sealed class XmlDoc
    {
        public List<string> Summary { get; set; } = [];
        public List<string> Returns { get; set; } = [];
        public List<XElement> Parameters { get; set; } = [];

        public string ToString(Dictionary<string,int> paramOrder, string indent)
        {
            var sb = new StringBuilder();

            if (Summary.Count > 0)
            {
                sb.AppendLine($"{indent}/// <summary>");

                for (var i = 0; i < Summary.Count; i++)
                {
                    var s = Summary[i];
                    sb.AppendLine($"{indent}/// {s}");
                    if (i < Summary.Count - 1) sb.AppendLine($"{indent}/// <br/>");
                }

                sb.AppendLine($"{indent}/// </summary>");
            }

            sb.AppendLine($"{indent}/// <param name=\"value\">The value to convert.</param>");

            var parameters = Parameters.OrderBy(x =>
                                       {
                                           var name = (string?) x.Attribute("name");
                                           return paramOrder.TryGetValue(name ?? "", out var idx) ? idx : int.MaxValue;
                                       })
                                       .Select(p => p.ToString())
                                       .Where(s => !string.IsNullOrEmpty(s))
                                       .ToList();

            foreach (var line in parameters.Select(s => s.Split(["\r\n", "\n"], StringSplitOptions.None))
                                           .SelectMany(lines => lines))
            {
                sb.AppendLine($"{indent}/// {line}");
            }

            if (Returns.Count > 0)
            {
                foreach (var line in Returns.Select(s => s.Split(["\r\n", "\n"], StringSplitOptions.None))
                                            .SelectMany(lines => lines))
                {
                    sb.AppendLine($"{indent}/// {line}");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}