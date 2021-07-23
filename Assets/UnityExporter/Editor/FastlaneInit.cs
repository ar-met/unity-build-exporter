using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityExporter
{
    /// <summary>
    ///     Provides <see cref="UnityEditor.MenuItem" /> entries to help initialize directories that work with
    ///     the 'fastlane plugin'
    ///     <a href="https://github.com/ar-met/fastlane-plugin-unity-exporter">'unity-exporter'</a>.
    /// </summary>
    internal static class FastlaneInit
    {
        private const string k_DefaultBasePath = "fastlane-build-exporter";
        private const string k_GitKeep         = ".gitkeep";

        private static string GetFastlaneExportParentDirectory(BuildTarget buildTarget)
        {
            return Path.Combine(
                Application.dataPath,
                "..",
                k_DefaultBasePath,
                buildTarget.ToString());
        }

        private static string GetFastlaneExportGitKeepFile(BuildTarget buildTarget)
        {
            return Path.Combine(GetFastlaneExportParentDirectory(buildTarget), k_GitKeep);
        }

        private static void PrintAdditionalInformation(BuildTarget buildTarget)
        {
            Debug.Log(
                $"Initialized 'fastlane' for '{buildTarget}'. "                                        +
                "Please commit your '.gitkeep' to add the created intermediate directories to 'git'. " +
                "Now navigate to the directory and 'fastlane init'. "                                  +
                "For more information on 'fastlane' see: "                                             +
                $"<color=cyan>https://docs.fastlane.tools/getting-started/{buildTarget.ToString().ToLower()}/setup/</color>."
            );

            Debug.Log(
                "After successful 'fastlane init', "                                 +
                "you now can add the 'fastlane' plugin 'unity_exporter' like this: " +
                "'fastlane add_plugin unity_exporter'. "                             +
                "For more information on the plugin see: "                           +
                "<color=cyan>https://github.com/ar-met/fastlane-plugin-unity-exporter</color>.");

            Application.OpenURL("file:///" + GetFastlaneExportParentDirectory(buildTarget));
        }

        private static bool ValidateFastlaneMenuItem(BuildTarget buildTarget)
        {
            bool notRestrictedByEditorPlatform =
                // the windows editor can only publish Android
                Application.platform == RuntimePlatform.WindowsEditor &&
                buildTarget          == BuildTarget.Android ||
                // the osx editor can publish both Android and iOS
                Application.platform == RuntimePlatform.OSXEditor &&
                (buildTarget == BuildTarget.Android || buildTarget == BuildTarget.iOS);

            return notRestrictedByEditorPlatform && !File.Exists(GetFastlaneExportGitKeepFile(buildTarget));
        }

        private static void InitializeFastlane(BuildTarget buildTarget)
        {
            Directory.CreateDirectory(GetFastlaneExportParentDirectory(buildTarget));
            File.Create(GetFastlaneExportGitKeepFile(buildTarget));

            // TODO export build

            PrintAdditionalInformation(buildTarget);
        }

#region Android

        private const string k_MenuItemNameAndroid = "Initialize 'fastlane' directories for Android";

        [MenuItem(Constants.k_MenuItemBaseName + k_MenuItemNameAndroid)]
        private static void InitializeFastlaneForAndroid()
        {
            InitializeFastlane(BuildTarget.Android);
        }

        [MenuItem(Constants.k_MenuItemBaseName + k_MenuItemNameAndroid, true)]
        private static bool ValidateInitializeFastlaneForAndroid()
        {
            return ValidateFastlaneMenuItem(BuildTarget.Android);
        }

#endregion

#region iOS

        private const string k_MenuItemNameIOS = "Initialize 'fastlane' directories for iOS";

        [MenuItem(Constants.k_MenuItemBaseName + k_MenuItemNameIOS)]
        private static void InitializeFastlaneForIOS()
        {
            InitializeFastlane(BuildTarget.iOS);
        }

        [MenuItem(Constants.k_MenuItemBaseName + k_MenuItemNameIOS, true)]
        private static bool ValidateInitializeFastlaneForIOS()
        {
            return ValidateFastlaneMenuItem(BuildTarget.iOS);
        }

#endregion
    }
}
