using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityExporter
{
    /// <summary>
    ///     Simple semantic version helper. See: https://semver.org/
    /// </summary>
    // TODO improve: might not be the cleanest implementation, but it works
    internal class SemanticVersion
    {
        public static bool TryParse(string versionString, out SemanticVersion semanticVersion)
        {
            semanticVersion = new SemanticVersion();
            try
            {
                var semVersionPattern = new Regex(@"(-?\d+)\.(-?\d+)\.(-?\d+)");
                var match             = semVersionPattern.Match(versionString);

                // 1) the whole version "{major}.{minor}.{patch}"
                // 2) major
                // 3) minor
                // 4) patch
                // == 4
                if (match.Groups.Count != 4)
                {
                    throw new Exception("Not a valid semantic version string.");
                }

                if (!uint.TryParse(match.Groups[1].ToString(), out uint major) ||
                    !uint.TryParse(match.Groups[2].ToString(), out uint minor) ||
                    !uint.TryParse(match.Groups[3].ToString(), out uint patch))
                {
                    throw new Exception("Cannot parse major/minor/patch. Not a valid semantic version string.");
                }

                semanticVersion.major = major;
                semanticVersion.minor = minor;
                semanticVersion.patch = patch;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(SemanticVersion)}: Cannot parse version string '{versionString}'. {e}");
            }

            return false;
        }

        public uint major { get; private set; }
        public uint minor { get; private set; }
        public uint patch { get; private set; }

        public override string ToString()
        {
            return $"{major}.{minor}.{patch}";
        }
    }
}
