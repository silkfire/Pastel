using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace SimpleTypesExtensionsGenerator;

public sealed class DocumentationFromXmlFile
{
    private readonly XDocument? xdoc;

    public DocumentationFromXmlFile(Compilation compilation)
    {
        var runtimeRef = compilation.References
                                    .Select(compilation.GetAssemblyOrModuleSymbol)
                                    .OfType<IAssemblySymbol>()
                                    .FirstOrDefault(a => a.Name == "System.Runtime");

        if (runtimeRef == null) return;

        var referencePath = compilation.References
                                       .FirstOrDefault(r =>
                                       {
                                           var asm = compilation.GetAssemblyOrModuleSymbol(r) as IAssemblySymbol;
                                           return asm?.Name == "System.Runtime";
                                       })
                                       ?.Display;

        if (string.IsNullOrEmpty(referencePath)) return;

        try
        {
            var xmlPath = Path.ChangeExtension(referencePath, ".xml");
            xdoc = XDocument.Load(xmlPath);
        }
        catch
        {

        }
    }

    public string? GetDocumentationForBclMethod(IMethodSymbol method)
    {
        if (xdoc is null) return null;

        var docId = method.GetDocumentationCommentId();

        if (docId is null) return null;

        try
        {
            var member = xdoc.Descendants("member")
                             .FirstOrDefault(m => string.Equals(m.Attribute("name")?.Value, docId, StringComparison.Ordinal));

            return member?.ToString();
        }
        catch
        {
            return null;
        }
    }
}