using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Pastel.Tests
{
    public class EnvironmentTests
    {
        private static readonly object _lock = new object();

        [Theory]
        [InlineData("BITBUCKET_SOMEKEY", "somevalue",             true)]
        [InlineData("TEAMCITY_SOMEKEY",  "somevalue",             true)]
        [InlineData("NO_COLOR",          "true",                  true)]
        [InlineData("GITHUB_ACTION",     "somevalue",             true)]
        [InlineData("CI",                "true",                  true)]
        [InlineData("CI",                "1",                     true)]
        [InlineData("CI",                "false",                 false)]
        [InlineData("CI",                "0",                     false)]
        [InlineData("JENKINS_URL",       "http://localhost:8080", true)]
        [InlineData("TEAMCITY_VERSION",  "2023.1.1",              true)]
        [InlineData("SOME_OTHER_KEY",    "somevalue",             false)]

        public void TestEnvironmentVariables(string key, string value, bool expected)
        {
            lock (_lock)
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
}
