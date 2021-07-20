using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UnityExporter
{
    /// <summary>
    ///     Note that the 'PreloadedAssets' change during build. To not mess with Git, we cache them before build
    ///     and restore them afterwards.
    ///     This is especially useful when using ARFoundation, ARCore and ARKit,
    ///     because these packages change the 'PreloadedAssets'.
    /// </summary>
    // We need to combine build-interface implementation with -attribute,
    // because we require two different values for the callback order.
    internal class PreloadedAssetsBuildHandler : IPreprocessBuildWithReport
    {
        private static Object[] s_CachedPreloadedAssetsBeforeBuild;

#region Postprocess

        [PostProcessBuild(42)]
        public static void PostProcess(BuildTarget buildTarget, string pathToBuiltProject)
        {
            Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets().Where(x => x != null).ToArray();
            Debug.Log("PreloadedAssets after build: " + $"'{preloadedAssets.Select(x => x.name).ElementsToString()}'");

            // Debug.Log(
            //     "PreloadedAssets: " +
            //     $"Setting to '{s_CachedPreloadedAssetsBeforeBuild.Select(x => x.name).ElementsToString()}'");

            PlayerSettings.SetPreloadedAssets(s_CachedPreloadedAssetsBeforeBuild);
            AssetDatabase.SaveAssets(); // now applying the changes of this script

            preloadedAssets = PlayerSettings.GetPreloadedAssets().Where(x => x != null).ToArray();
            Debug.Log("PreloadedAssets after reset: " + $"'{preloadedAssets.Select(x => x.name).ElementsToString()}'");
        }

#endregion

#region Preprocess

        /// <summary>
        ///     Must be lower than "UnityEditor.XR.Management.XRGeneralBuildProcessor.callbackOrder".
        /// </summary>
        public int callbackOrder => -42; // do not use 'int.MinValue' as it will not be registered correctly

        public void OnPreprocessBuild(BuildReport report)
        {
            s_CachedPreloadedAssetsBeforeBuild = PlayerSettings.GetPreloadedAssets().Where(x => x != null).ToArray();
            Debug.Log(
                "PreloadedAssets before build: " +
                $"'{s_CachedPreloadedAssetsBeforeBuild.Select(x => x.name).ElementsToString()}'");
        }

#endregion
    }
}
