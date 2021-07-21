using System;
using UnityEditor;
using UnityEngine;

namespace UnityExporter
{
    /// <summary>
    ///     Helps managing version and version-code/build-number in <see cref="PlayerSettings" />.
    /// </summary>
    internal class PlayerSettingsVersioner
    {
        public enum Bump
        {
            Major,
            Minor,
            Patch
        }

        public PlayerSettingsVersioner(string newVersion, string newVersionCode)
        {
            if (!string.IsNullOrEmpty(newVersion))
            {
                if (Enum.TryParse(newVersion, true, out Bump bump))
                {
                    if (SemanticVersion.TryParse(PlayerSettings.bundleVersion, out var semanticVersion))
                    {
                        switch (bump)
                        {
                            case Bump.Major:
                                semanticVersion.major++;
                                break;
                            case Bump.Minor:
                                semanticVersion.minor++;
                                break;
                            case Bump.Patch:
                                semanticVersion.patch++;
                                break;
                        }

                        this.newVersion = semanticVersion;
                    }
                }
                else if (SemanticVersion.TryParse(newVersion, out var semanticVersion))
                {
                    this.newVersion = semanticVersion;
                }
            }
            else
            {
                if (SemanticVersion.TryParse(PlayerSettings.bundleVersion, out SemanticVersion semanticVersion))
                {
                    this.newVersion = semanticVersion;
                }
            }

            if (!string.IsNullOrEmpty(newVersionCode))
            {
                if (bool.TryParse(newVersionCode, out bool increment) && increment)
                {
                    this.newVersionCode = currentVersionCode + 1;
                }
                else if (int.TryParse(newVersionCode, out int newVersionCodeAsInt) && newVersionCodeAsInt >= 0)
                {
                    this.newVersionCode = newVersionCodeAsInt;
                }
            }
            else
            {
                this.newVersionCode = currentVersionCode;
            }
        }

        public SemanticVersion newVersion     { get; }
        public int             newVersionCode { get; }

        private int currentVersionCode
        {
            get
            {
                if (int.TryParse(PlayerSettings.iOS.buildNumber, out int iosCode))
                {
                    return Mathf.Max(
                        PlayerSettings.Android.bundleVersionCode,
                        iosCode);
                }

                return PlayerSettings.Android.bundleVersionCode;
            }
        }

        public void Apply()
        {
            PlayerSettings.bundleVersion             = newVersion.ToString();
            PlayerSettings.Android.bundleVersionCode = newVersionCode;
            PlayerSettings.iOS.buildNumber           = newVersionCode.ToString();
        }

        /// <summary>
        ///     Validates the currently set <see cref="PlayerSettings.bundleVersion" /> and
        ///     <see cref="PlayerSettings.Android.bundleVersionCode" /> and
        ///     <see cref="PlayerSettings.iOS.buildNumber" />.
        /// </summary>
        /// <returns>'true' if valid, 'false' otherwise.</returns>
        public static bool IsValid()
        {
            return
                SemanticVersion.TryParse(PlayerSettings.bundleVersion, out _) &&
                PlayerSettings.Android.bundleVersionCode >= 0                 &&
                int.TryParse(PlayerSettings.iOS.buildNumber, out int iosCode) &&
                iosCode >= 0;
        }
    }
}
