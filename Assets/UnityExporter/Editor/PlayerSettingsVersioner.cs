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

        private PlayerSettingsVersioner() { }

        public SemanticVersion version     { get; private set; }
        public int             versionCode { get; private set; }

        private static int currentVersionCode
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
            PlayerSettings.bundleVersion             = version.ToString();
            PlayerSettings.Android.bundleVersionCode = versionCode;
            PlayerSettings.iOS.buildNumber           = versionCode.ToString();
        }

        public static bool TryParse(
            string newVersion, string newVersionCode, out PlayerSettingsVersioner playerSettingsVersioner)
        {
            if (!IsValid(out var currentVersion))
            {
                playerSettingsVersioner = null;
                return false;
            }

            playerSettingsVersioner = new PlayerSettingsVersioner
            {
                version     = currentVersion,
                versionCode = currentVersionCode
            };

            if (string.IsNullOrEmpty(newVersion) || string.IsNullOrEmpty(newVersionCode))
            {
                return true;
            }

            // new version
            if (Enum.TryParse(newVersion, true, out Bump bump))
            {
                switch (bump)
                {
                    case Bump.Major:
                        playerSettingsVersioner.version.major++;
                        break;
                    case Bump.Minor:
                        playerSettingsVersioner.version.minor++;
                        break;
                    case Bump.Patch:
                        playerSettingsVersioner.version.patch++;
                        break;
                }
            }
            else if (SemanticVersion.TryParse(newVersion, out var newSemanticVersion))
            {
                playerSettingsVersioner.version = newSemanticVersion;
            }
            else
            {
                Debug.LogError($"Cannot parse new version '{newVersion}'.");
                return false;
            }

            // version code
            if (string.Equals("increment", newVersionCode))
            {
                playerSettingsVersioner.versionCode = currentVersionCode + 1;
            }
            else if (int.TryParse(newVersionCode, out int newVersionCodeAsInt) && newVersionCodeAsInt >= 0)
            {
                playerSettingsVersioner.versionCode = newVersionCodeAsInt;
            }
            else
            {
                Debug.LogError($"Cannot parse new version code '{newVersionCode}'.");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Validates the currently set <see cref="PlayerSettings.bundleVersion" /> and
        ///     <see cref="PlayerSettings.Android.bundleVersionCode" /> and
        ///     <see cref="PlayerSettings.iOS.buildNumber" />.
        /// </summary>
        /// <returns>'true' if valid, 'false' otherwise.</returns>
        public static bool IsValid(out SemanticVersion currentVersion)
        {
            return
                SemanticVersion.TryParse(PlayerSettings.bundleVersion, out currentVersion) &&
                PlayerSettings.Android.bundleVersionCode >= 0                              &&
                int.TryParse(PlayerSettings.iOS.buildNumber, out int iosCode)              &&
                iosCode >= 0;
        }
    }
}
