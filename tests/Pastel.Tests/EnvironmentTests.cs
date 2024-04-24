namespace Pastel.Tests
{
    using Xunit;

    using System;
    using System.Collections.Generic;

    public class EnvironmentTests
    {
        private const string DisableEnvironmentDetectionEnvironmentVariableName = "PASTEL_DISABLE_ENVIRONMENT_DETECTION";

        [Theory, CombinatorialData]
        public void TestEnvironmentVariables([CombinatorialMemberData(nameof(GetEnvironmentVariables))] (string Key, string Value, bool ExpectedOutcome) environmentVariable, [CombinatorialMemberData(nameof(GetEnvironmentDetectionDisabledEnvironmentVariables))] string environmentDetectionDisabledEnvironmentVariable)
        {
            try
            {
                // Arrange
                if (environmentDetectionDisabledEnvironmentVariable != null)
                {
                    Environment.SetEnvironmentVariable(DisableEnvironmentDetectionEnvironmentVariableName, environmentDetectionDisabledEnvironmentVariable, EnvironmentVariableTarget.Process);
                }
                Environment.SetEnvironmentVariable(environmentVariable.Key, environmentVariable.Value, EnvironmentVariableTarget.Process);

                // Act
                var result = EnvironmentDetector.ColorsEnabled();

                // Assert
                Assert.Equal(environmentDetectionDisabledEnvironmentVariable != null || environmentVariable.ExpectedOutcome, result);
            }
            finally
            {
                // Cleanup
                Environment.SetEnvironmentVariable(DisableEnvironmentDetectionEnvironmentVariableName, null, EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable(environmentVariable.Key, null, EnvironmentVariableTarget.Process);
            }
        }

        private static IEnumerable<(string Key, string Value, bool ExpectedOutcome)> GetEnvironmentVariables()
        {
            yield return ("BITBUCKET_SOMEKEY", "somevalue",             false);
            yield return ("BITbucket_SOMEKEY", "somevalue",             false);
            yield return ("TEAMCITY_SOMEKEY",  "somevalue",             false);
            yield return ("TEAMcity_SOMEKEY",  "somevalue",             false);
            yield return ("NO_COLOR",          "true",                  false);
            yield return ("no_color",          "true",                  false);
            yield return ("GITHUB_ACTION",     "somevalue",             false);
            yield return ("GitHuB_action",     "somevalue",             false);
            yield return ("CI",                "true",                  false);
            yield return ("ci",                "true",                  false);
            yield return ("CI",                "1",                     false);
            yield return ("ci",                "1",                     false);
            yield return ("CI",                "false",                  true);
            yield return ("ci",                "false",                  true);
            yield return ("CI",                "0",                      true);
            yield return ("ci",                "0",                      true);
            yield return ("JENKINS_URL",       "http://localhost:8080", false);
            yield return ("jenkins_URL",       "http://localhost:8080", false);
            yield return ("TEAMCITY_VERSION",  "2023.1.1",              false);
            yield return ("teamcity_VERSION",  "2023.1.1",              false);
            yield return ("SOME_OTHER_KEY",    "somevalue",              true);
            yield return ("some_other_KEY",    "somevalue",              true);
        }

        private static IEnumerable<string> GetEnvironmentDetectionDisabledEnvironmentVariables()
        {
            yield return null;
            yield return "1";
            yield return "true";
            yield return "TRUE";
            yield return "trUe";
            yield return "something_random";
        }
    }
}
