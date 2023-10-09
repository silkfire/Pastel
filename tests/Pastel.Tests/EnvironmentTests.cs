using Xunit;

using System;

namespace Pastel.Tests
{
    public class EnvironmentTests
    {
        private static readonly object _lock = new object();

        [Theory]
        [InlineData("BITBUCKET_SOMEKEY", "somevalue",             true)]
        [InlineData("BITbucket_SOMEKEY", "somevalue",             true)]
        [InlineData("TEAMCITY_SOMEKEY",  "somevalue",             true)]
        [InlineData("TEAMcity_SOMEKEY",  "somevalue",             true)]
        [InlineData("NO_COLOR",          "true",                  true)]
        [InlineData("no_color",          "true",                  true)]
        [InlineData("GITHUB_ACTION",     "somevalue",             true)]
        [InlineData("GitHuB_action",     "somevalue",             true)]
        [InlineData("CI",                "true",                  true)]
        [InlineData("ci",                "true",                  true)]
        [InlineData("CI",                "1",                     true)]
        [InlineData("ci",                "1",                     true)]
        [InlineData("CI",                "false",                 false)]
        [InlineData("ci",                "false",                 false)]
        [InlineData("CI",                "0",                     false)]
        [InlineData("ci",                "0",                     false)]
        [InlineData("JENKINS_URL",       "http://localhost:8080", true)]
        [InlineData("jenkins_URL",       "http://localhost:8080", true)]
        [InlineData("TEAMCITY_VERSION",  "2023.1.1",              true)]
        [InlineData("teamcity_VERSION",  "2023.1.1",              true)]
        [InlineData("SOME_OTHER_KEY",    "somevalue",             false)]
        [InlineData("some_other_KEY",    "somevalue",             false)]

        public void TestEnvironmentVariables(string key, string value, bool expected)
        {
            try
            {
                // Arrange
                Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.Process);

                // Act
                var result = EnvironmentDetector.ColorsDisabled;

                // Assert
                Assert.Equal(expected, result);
            }
            finally
            {
                // Cleanup
                Environment.SetEnvironmentVariable(key, null, EnvironmentVariableTarget.Process);
            }
        }
    }
}
