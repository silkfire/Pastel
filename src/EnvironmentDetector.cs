
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Pastel.Tests")]

namespace Pastel
{
    internal static class EnvironmentDetector
    {
        /// <summary>
        /// Returns true if the environment variables indicate that colors should be disabled.
        /// </summary>
        public static bool ColorsDisabled => HasEnvironmentVariable((key, value) => _environmentVariableDetectors.Any(x => x(key, value)));

        private static Func<string, string, bool>[] _environmentVariableDetectors = new Func<string, string, bool>[]
        {
            IsBitbucketEnvironmentVariableKey,
            IsTeamCityEnvironmentVariableKey,
            NoColor,
            IsGitHubAction,
            IsCI,
            IsJenkins,
            IsTeamCity
        };

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
            return key.Equals("CI", StringComparison.OrdinalIgnoreCase)
                && (value.Equals("true", StringComparison.OrdinalIgnoreCase)
                || value.Equals("1", StringComparison.OrdinalIgnoreCase));
        }

        //isjenkins
        private static bool IsJenkins(string key, string value)
        {
            return key.StartsWith("JENKINS_URL", StringComparison.OrdinalIgnoreCase);
        }


        // is TEAMCITY_VERSION 
        private static bool IsTeamCity(string key, string value)
        {
            return key.StartsWith("TEAMCITY_VERSION", StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasEnvironmentVariable(Func<string, string, bool> predicate)
        {
            var processKeys = EnumerateEnvironmentVariables(EnvironmentVariableTarget.Process);
            var userKeys = EnumerateEnvironmentVariables(EnvironmentVariableTarget.User);
            var machineKeys = EnumerateEnvironmentVariables(EnvironmentVariableTarget.Machine);

            return processKeys
                .Concat(userKeys)
                .Concat(machineKeys)
                .Any(x => predicate(x.Key, x.Value));
        }

        private static IEnumerable<KeyValuePair> EnumerateEnvironmentVariables(EnvironmentVariableTarget target)
        {
            foreach (var entry in Environment.GetEnvironmentVariables(target).OfType<DictionaryEntry>())
            {
                yield return new KeyValuePair(entry.Key.ToString(), entry.Value?.ToString() ?? string.Empty);
            }
        }
        private struct KeyValuePair
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public KeyValuePair(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}