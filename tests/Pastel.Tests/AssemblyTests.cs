namespace Pastel.Tests
{
    using Xunit;

    using System.Linq;
    using System.Reflection;
#if NET8_0_OR_GREATER
    using System.Runtime.CompilerServices;
#endif

    public class AssemblyTests
    {
#if NET8_0_OR_GREATER
        [Fact]
        public void The_Assembly_Should_Have_Runtime_Marshalling_Disabled()
        {
            // Guards against the attribute silently not being applied, which is what happened
            // while its ItemGroup was conditioned on $(DefineConstants) instead of $(TargetFramework)

            var disableRuntimeMarshallingAttribute = typeof(ConsoleExtensions).Assembly.GetCustomAttribute<DisableRuntimeMarshallingAttribute>();

            Assert.NotNull(disableRuntimeMarshallingAttribute);
        }
#else
        [Fact]
        public void The_Assembly_Should_Not_Have_Runtime_Marshalling_Disabled()
        {
            // The attribute doesn't exist on .NET Framework, so the assembly must not carry it

            var disableRuntimeMarshallingAttribute = typeof(ConsoleExtensions).Assembly
                                                                              .GetCustomAttributes()
                                                                              .FirstOrDefault(a => a.GetType().FullName == "System.Runtime.CompilerServices.DisableRuntimeMarshallingAttribute");

            Assert.Null(disableRuntimeMarshallingAttribute);
        }
#endif
    }
}
