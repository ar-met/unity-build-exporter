using System.IO;
using UnityEditor;
using UnityEngine;

namespace armet.BuildExporter
{
    /// <summary>
    ///     Provides <see cref="UnityEditor.MenuItem" /> entries to help initialize directories that work with
    ///     the 'fastlane plugin'
    ///     <a href="https://github.com/ar-met/fastlane-plugin-unity-exporter">'unity-exporter'</a>.
    /// </summary>
    internal static class FastlaneInit
    {
        /// <summary>
        ///     Note that the 'fastlane plugin' expects this path.
        /// </summary>
        private const string k_DefaultBasePath = "fastlane-build-exporter";

        private const string k_GitKeep = ".gitkeep";

        private const string k_GitignoreFastlaneExporterFiles =
            @"
# fastlane plugin unity exporter
fastlane-build-exporter/*/unity-export/*
fastlane-build-exporter/*/fastlane/report.xml
fastlane-build-exporter/*/.bundle
fastlane-build-exporter/**/*.log
";

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

        private static string GetFastlaneExportUnityBuildDirectory(BuildTarget buildTarget)
        {
            return Path.Combine(GetFastlaneExportParentDirectory(buildTarget), "unity-export");
        }

        private static void PrintAdditionalInformation(BuildTarget buildTarget)
        {
            Debug.Log(
                $"Initialized 'fastlane' for '{buildTarget}'. " +
                "Please commit your '.gitignore' and '.gitkeep' via 'git'.");

            Debug.Log(
                $"Now 'fastlane init' in directory '{GetFastlaneExportParentDirectory(buildTarget)}'. " +
                "For more information on 'fastlane' see: "                                              +
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

            return notRestrictedByEditorPlatform
                // when developing we always want to be able to start the initialization
#if !UNITY_EXPORTER_DEV
                   && !File.Exists(GetFastlaneExportGitKeepFile(buildTarget))
#endif
                ;
        }

        private static void AppendToGitIgnore()
        {
            string gitignorePath = Path.Combine(Application.dataPath, "..", ".gitignore");
            if (!File.Exists(gitignorePath))
                File.Create(gitignorePath);

            string gitignoreContent = File.ReadAllText(gitignorePath);
            if (!gitignoreContent.Contains(k_GitignoreFastlaneExporterFiles))
                File.AppendAllText(gitignorePath, k_GitignoreFastlaneExporterFiles);
        }

        private static void InitializeFastlane(BuildTarget buildTarget)
        {
            Directory.CreateDirectory(GetFastlaneExportParentDirectory(buildTarget));
            File.Create(GetFastlaneExportGitKeepFile(buildTarget));

            // TODO refactor BuildUtility such that we can use the CreateBuild method (?)
            bool tmpDevelopment                  = EditorUserBuildSettings.development;
            bool tmpExportAsGoogleAndroidProject = EditorUserBuildSettings.exportAsGoogleAndroidProject;
            EditorUserBuildSettings.development                  = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            BuildPipeline.BuildPlayer(
                new string[0],
                GetFastlaneExportUnityBuildDirectory(buildTarget),
                buildTarget,
                BuildOptions.None);

            EditorUserBuildSettings.development                  = tmpDevelopment;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = tmpExportAsGoogleAndroidProject;

            PrintAdditionalInformation(buildTarget);
            AppendToGitIgnore();
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
