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
        private static EditorUserSettings s_EditorUserSettings;
        private static BuildArguments     s_BuildArguments;
        private static string[]           s_ScenesInBuild;

        private static void InitBuild()
        {
            Debug.Log($"{nameof(BuildUtility)}.{nameof(InitBuild)}");

            s_EditorUserSettings = new EditorUserSettings();
            Debug.Log($"{nameof(EditorUserSettings)} -- {s_EditorUserSettings}");

            s_BuildArguments = new BuildArguments();
            Debug.Log($"{nameof(BuildArguments)} -- {s_BuildArguments}");

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
            s_EditorUserSettings.Apply();
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
            if (!PlayerSettingsVersioner.IsValid(out _))
            {
                Debug.LogError(
                    $"{nameof(PlayerSettings)} not valid for this script. " +
                    $"Verify that the '{nameof(PlayerSettings.bundleVersion)}' conforms to semantic versioning ({PlayerSettings.bundleVersion}). " +
                    $"Verify that Android's version code ({PlayerSettings.Android.bundleVersionCode}) and " +
                    $"iOS' build number ({PlayerSettings.iOS.buildNumber}) are positive integers.");
                return;
            }

            InitBuild();

            EditorUserBuildSettings.development                  = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            if (PlayerSettingsVersioner.TryParse(
                s_BuildArguments[BuildArguments.k_Version],
                s_BuildArguments[BuildArguments.k_VersionCode],
                out PlayerSettingsVersioner playerSettingsVersioner))
            {
                playerSettingsVersioner.Apply();
            }
            else
            {
                Debug.LogError($"Cannot parse {nameof(PlayerSettings)} and/or version/version-code.");
                return;
            }

            if (!string.IsNullOrEmpty(s_BuildArguments[BuildArguments.k_ExportPath]))
            {
                // Note that the following is set via batchmode parameter "buildTarget":
                // - "UNITY_ANDROID", "UNITY_IOS", ...
                // - EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.selectedBuildTargetGroup, ...

                Directory.CreateDirectory(s_BuildArguments[BuildArguments.k_ExportPath]);
                Debug.Log(
                    $"{nameof(CreateBuild)}.{s_EditorUserSettings.buildTarget}: Starting build now. " +
                    $"Export path '{s_BuildArguments[BuildArguments.k_ExportPath]}'. "                +
                    $"Define Symbols '{PlayerSettings.GetScriptingDefineSymbolsForGroup(s_EditorUserSettings.buildTargetGroup)}'");

                var report = BuildPipeline.BuildPlayer(
                    s_ScenesInBuild,
                    s_BuildArguments[BuildArguments.k_ExportPath],
                    s_EditorUserSettings.buildTarget,
                    BuildOptions.None);

                Debug.Log(
                    $"{nameof(BuildUtility)}.{nameof(CreateBuild)}.{s_EditorUserSettings.buildTarget}.Report: " +
                    $"{ReportToString(report)}");
            }

            // finish
            FinishBuild();
        }
    }
}
