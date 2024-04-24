namespace Pastel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal static class EnvironmentDetector
    {
        private const string DisableEnvironmentDetectionEnvironmentVariableName = "PASTEL_DISABLE_ENVIRONMENT_DETECTION";

        /// <summary>
        /// Returns <see langword="true"/> if at least one of a predefined set of environment variables are set. These environment variables could e.g. indicate that the application is running in a CI/CD environment.
        /// </summary>
        public static bool ColorsEnabled()
        {
            return Environment.GetEnvironmentVariable(DisableEnvironmentDetectionEnvironmentVariableName) != null || !HasEnvironmentVariable();
        }

#if NET7_0_OR_GREATER
        private static readonly Func<string, string, bool>[] s_environmentVariableDetectors = [
                                                                                                 IsBitbucketEnvironmentVariableKey,
                                                                                                 IsTeamCityEnvironmentVariableKey,
                                                                                                 NoColor,
                                                                                                 IsGitHubAction,
                                                                                                 IsCI,
                                                                                                 IsJenkins
                                                                                              ];
#else
        private static readonly Func<string, string, bool>[] s_environmentVariableDetectors = {
                                                                                                 IsBitbucketEnvironmentVariableKey,
                                                                                                 IsTeamCityEnvironmentVariableKey,
                                                                                                 NoColor,
                                                                                                 IsGitHubAction,
                                                                                                 IsCI,
                                                                                                 IsJenkins
                                                                                              };
#endif

        private static bool IsBitbucketEnvironmentVariableKey(string key, string value)
        {
            return key.StartsWith("BITBUCKET_", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsTeamCityEnvironmentVariableKey(string key, string value)
        {
            return key.StartsWith("TEAMCITY_", StringComparison.OrdinalIgnoreCase);
        }

        // https://no-color.org/
        private static bool NoColor(string key, string value)
        {
            return key.Equals("NO_COLOR", StringComparison.OrdinalIgnoreCase);
        }

        // Set by GitHub Actions
        private static bool IsGitHubAction(string key, string value)
        {
            return key.StartsWith("GITHUB_ACTION", StringComparison.OrdinalIgnoreCase);
        }

        // Set by GitHub Actions and Travis CI
        private static bool IsCI(string key, string value)
        {
            return     key.Equals("CI", StringComparison.OrdinalIgnoreCase)
                   && (value.Equals("true", StringComparison.OrdinalIgnoreCase) || value == "1");
        }

        // Detect Jenkins enviroment
        private static bool IsJenkins(string key, string value)
        {
            return key.StartsWith("JENKINS_URL", StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasEnvironmentVariable()
        {
            var environmentVariables = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process).Cast<DictionaryEntry>()
                                                  .Concat(Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User).Cast<DictionaryEntry>())
                                                  .Concat(Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine).Cast<DictionaryEntry>())
                                                  .Select(de => new KeyValuePair<string, string>(de.Key.ToString(), de.Value != null ? de.Value.ToString() : ""))
                                                  .GroupBy(kvp => kvp.Key, (_, kvps) => kvps.First())
                                                  .ToList();

            foreach (var environmentVariable in environmentVariables)
            {
                if (s_environmentVariableDetectors.Any(evd => evd(environmentVariable.Key, environmentVariable.Value)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
