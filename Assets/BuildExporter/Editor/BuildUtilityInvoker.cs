using UnityEditor;

namespace armet.BuildExporter
{
    internal static class BuildUtilityInvoker
    {
#if UNITY_EXPORTER_DEV
        [MenuItem(Constants.k_MenuItemBaseName + nameof(BuildCurrentTarget))]
        private static void BuildCurrentTarget()
        {
            if (EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Android &&
                EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.iOS)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }

            BuildUtility.CreateBuild();
        }
#endif
    }
}
