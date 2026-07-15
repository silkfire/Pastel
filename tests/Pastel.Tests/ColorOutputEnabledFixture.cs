namespace Pastel.Tests
{
    using Xunit;

    /// <summary>
    /// Enables color output for every test that asserts on colored output.
    /// <para>Pastel disables itself when it detects a CI/CD environment, so without this the assertions would fail on a build server while passing locally.</para>
    /// </summary>
    public class ColorOutputEnabledFixture
    {
        public ColorOutputEnabledFixture()
        {
            ConsoleExtensions.Enable();
        }
    }

    [CollectionDefinition(Name)]
    public class ColorOutputEnabledCollection : ICollectionFixture<ColorOutputEnabledFixture>
    {
        public const string Name = "Color output enabled";
    }
}
