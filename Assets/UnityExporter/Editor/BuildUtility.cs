using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace UnityExporter
{
    public static class BuildUtility
    {
        private static string[] s_CurrentCommandLineArguments;
        private static string   s_TargetBuildPathAndroid;
        private static string   s_TargetBuildPathIOS;

        private static string[] s_ScenesInBuild;

        private static string s_NewVersion;
        private static bool   s_IncrementVersionCode;
        private static bool   s_IsBatchmode;

        private static SettingsCopy s_Settings;

        private static void InitBuild()
        {
            Debug.Log($"{nameof(BuildUtility)}.{nameof(InitBuild)}");
            s_Settings                    = SettingsCopy.Create();
            s_CurrentCommandLineArguments = Environment.GetCommandLineArgs();
            Debug.Log(
                $"{nameof(BuildUtility)}.{nameof(InitBuild)}." +
                $"CommandlineArgs: '{s_CurrentCommandLineArguments.ElementsToString()}'");

            for (int i = 0; i < s_CurrentCommandLineArguments.Length; i++)
            {
                if (s_CurrentCommandLineArguments[i].Contains("pathIOS"))
                {
                    s_TargetBuildPathIOS = s_CurrentCommandLineArguments[++i];
                }
                else if (s_CurrentCommandLineArguments[i].Contains("pathAndroid"))
                {
                    s_TargetBuildPathAndroid = s_CurrentCommandLineArguments[++i];
                }
                else if (s_CurrentCommandLineArguments[i].Contains("incrementVersionCode"))
                {
                    s_IncrementVersionCode = true;
                }
                else if (s_CurrentCommandLineArguments[i].Contains("batchmode"))
                {
                    s_IsBatchmode = true;
                }
                else if (s_CurrentCommandLineArguments[i].Contains("newVersion"))
                {
                    s_NewVersion = s_CurrentCommandLineArguments[++i];
                }
            }

            Debug.Log($"{nameof(BuildUtility)}.{nameof(InitBuild)}.newVersion: "           + s_NewVersion);
            Debug.Log($"{nameof(BuildUtility)}.{nameof(InitBuild)}.incrementVersionCode: " + s_IncrementVersionCode);

            // providing fallback only if we use this in the editor
            // no fallback when used during "batchmode"
            if (!s_IsBatchmode)
            {
                if (string.IsNullOrEmpty(s_TargetBuildPathAndroid))
                {
                    s_TargetBuildPathAndroid = Path.Combine(Application.dataPath, "..", "Builds", "android");
                }

                if (string.IsNullOrEmpty(s_TargetBuildPathIOS))
                {
                    s_TargetBuildPathIOS = Path.Combine(Application.dataPath, "..", "Builds", "ios");
                }
            }

            Debug.Log($"{nameof(BuildUtility)}.{nameof(InitBuild)}.Path.Android: " + s_TargetBuildPathAndroid);
            Debug.Log($"{nameof(BuildUtility)}.{nameof(InitBuild)}.Path.iOS: "     + s_TargetBuildPathIOS);

            s_ScenesInBuild = EditorBuildSettings
                              .scenes
                              .Where(
                                  x =>
                                      x.enabled                                                      &&
                                      !x.path.ToLowerInvariant().Contains("Demo".ToLowerInvariant()) &&
                                      !x.path.ToLowerInvariant().Contains("Test".ToLowerInvariant()))
                              .Select(x => x.path)
                              .ToArray();

            Debug.Log($"{nameof(BuildUtility)}.{nameof(InitBuild)}.Scenes: {s_ScenesInBuild.ElementsToString()}");
        }

        private static void FinishBuild()
        {
            Debug.Log($"{nameof(BuildUtility)}.{nameof(FinishBuild)}");
            s_Settings.Apply();
        }

        private static string ReportToString(BuildReport report)
        {
            return $"{report.name}, "                                                                            +
                   $"Path {report.summary.outputPath}, "                                                         +
                   $"Time {(int) report.summary.totalTime.TotalMinutes}:{report.summary.totalTime.Seconds:00}, " +
                   $"Number of errors {report.summary.totalErrors}";
        }

        public static void CreateBuild()
        {
            // init
            InitBuild();

            // generic
            EditorUserBuildSettings.development                  = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            if (!string.IsNullOrEmpty(s_NewVersion))
            {
                PlayerSettings.bundleVersion = s_NewVersion;
            }

            if (s_IncrementVersionCode)
            {
                PlayerSettings.Android.bundleVersionCode++;
                PlayerSettings.iOS.buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
            }

            // Android 
            if (!string.IsNullOrEmpty(s_TargetBuildPathAndroid))
            {
                // note that "UNITY_ANDROID" is set via batchmode parameter "buildTarget"
                Directory.CreateDirectory(s_TargetBuildPathAndroid);
                Debug.Log(
                    $"{nameof(CreateBuild)}.Android: Starting build now. " +
                    $"Define Symbols '{PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android)}'");
                var report = BuildPipeline.BuildPlayer(
                    s_ScenesInBuild,
                    s_TargetBuildPathAndroid,
                    BuildTarget.Android,
                    BuildOptions.None);
                Debug.Log(
                    $"{nameof(BuildUtility)}.{nameof(CreateBuild)}.Android.Report: " +
                    $"{ReportToString(report)}");
            }

            // iOS
            if (!string.IsNullOrEmpty(s_TargetBuildPathIOS))
            {
                // note that "UNITY_IOS" is set via batchmode parameter "buildTarget"
                Directory.CreateDirectory(s_TargetBuildPathIOS);
                Debug.Log(
                    $"{nameof(CreateBuild)}.iOS: Starting build now. " +
                    $"Define Symbols '{PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS)}'");
                var report = BuildPipeline.BuildPlayer(
                    s_ScenesInBuild,
                    s_TargetBuildPathIOS,
                    BuildTarget.iOS,
                    BuildOptions.None);

                Debug.Log(
                    $"{nameof(BuildUtility)}.{nameof(CreateBuild)}.iOS.Report: " +
                    $"{ReportToString(report)}");
            }

            // finish
            FinishBuild();
        }

        private class SettingsCopy
        {
            public BuildTarget      buildTarget               { get; private set; }
            public BuildTargetGroup buildTargetGroup          { get; private set; }
            public string           scriptingDefineSymbols    { get; private set; }
            public bool             developmentBuildFlag      { get; private set; }
            public bool             exportAsGoogleAndroidFlag { get; private set; }

            public void Apply()
            {
                EditorUserBuildSettings.development                  = developmentBuildFlag;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = exportAsGoogleAndroidFlag;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    buildTargetGroup, scriptingDefineSymbols);
            }

            public static SettingsCopy Create()
            {
                return new SettingsCopy
                {
                    buildTarget      = EditorUserBuildSettings.activeBuildTarget,
                    buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup,
                    scriptingDefineSymbols =
                        PlayerSettings.GetScriptingDefineSymbolsForGroup(
                            EditorUserBuildSettings.selectedBuildTargetGroup),
                    developmentBuildFlag      = EditorUserBuildSettings.development,
                    exportAsGoogleAndroidFlag = EditorUserBuildSettings.exportAsGoogleAndroidProject
                };
            }
        }
    }
}
