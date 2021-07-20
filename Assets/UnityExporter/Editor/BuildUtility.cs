using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace UnityExporter
{
    internal static class BuildUtility
    {
        private static string[] s_CurrentCommandLineArguments;
        private static string   s_ExportPath;

        private static string[] s_ScenesInBuild;

        private static string s_Version;
        private static string s_VersionCode;
        private static bool   s_IsBatchmode;

        private static SettingsCopy s_Settings;

        private static void InitBuild()
        {
            Debug.Log($"{nameof(BuildUtility)}.{nameof(InitBuild)}");
            s_Settings = SettingsCopy.Create();
            Debug.Log($"Settings: {s_Settings}");
            s_CurrentCommandLineArguments = Environment.GetCommandLineArgs();
            Debug.Log(
                $"{nameof(BuildUtility)}.{nameof(InitBuild)}." +
                $"CommandlineArgs: '{s_CurrentCommandLineArguments.ElementsToString()}'");

            for (int i = 0; i < s_CurrentCommandLineArguments.Length; i++)
            {
                if (s_CurrentCommandLineArguments[i].Contains("exportPath"))
                {
                    s_ExportPath = s_CurrentCommandLineArguments[++i];
                }
                else if (s_CurrentCommandLineArguments[i].Contains("batchmode"))
                {
                    s_IsBatchmode = true;
                }
                else if (s_CurrentCommandLineArguments[i].Contains("version"))
                {
                    s_Version = s_CurrentCommandLineArguments[++i];
                }
                else if (s_CurrentCommandLineArguments[i].Contains("versionCode"))
                {
                    s_VersionCode = s_CurrentCommandLineArguments[++i];
                }
            }

            Debug.Log($"{nameof(BuildUtility)}.{nameof(InitBuild)}.newVersion: "           + s_Version);
            Debug.Log($"{nameof(BuildUtility)}.{nameof(InitBuild)}.incrementVersionCode: " + s_VersionCode);
            Debug.Log($"{nameof(BuildUtility)}.{nameof(InitBuild)}.exportPath: "           + s_ExportPath);

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

            EditorUserBuildSettings.development                  = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            if (!string.IsNullOrEmpty(s_Version))
            {
                PlayerSettings.bundleVersion = s_Version;
            }

            // if (s_VersionCode)
            {
                PlayerSettings.Android.bundleVersionCode++;
                PlayerSettings.iOS.buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
            }

            if (!string.IsNullOrEmpty(s_ExportPath))
            {
                // Note that the following is set via batchmode parameter "buildTarget":
                // - "UNITY_ANDROID", "UNITY_IOS", ...
                // - EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.selectedBuildTargetGroup, ...
                Directory.CreateDirectory(s_ExportPath);
                Debug.Log(
                    $"{nameof(CreateBuild)}.{s_Settings.buildTarget}: Starting build now. " +
                    $"Define Symbols '{PlayerSettings.GetScriptingDefineSymbolsForGroup(s_Settings.buildTargetGroup)}'");
                var report = BuildPipeline.BuildPlayer(
                    s_ScenesInBuild,
                    s_ExportPath,
                    s_Settings.buildTarget,
                    BuildOptions.None);
                Debug.Log(
                    $"{nameof(BuildUtility)}.{nameof(CreateBuild)}.{s_Settings.buildTarget}.Report: " +
                    $"{ReportToString(report)}");
            }

            // finish
            FinishBuild();
        }
    }
}
