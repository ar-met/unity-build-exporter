using UnityEditor;

namespace UnityExporter
{
    internal class PlayerSettingsVersioner
    {
        // TODO
    }
    
    internal class SettingsCopy
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
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, scriptingDefineSymbols);
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

        public override string ToString()
        {
            return
                $"{nameof(buildTarget)}: {buildTarget}, "                       +
                $"{nameof(buildTargetGroup)}: {buildTargetGroup}, "             +
                $"{nameof(scriptingDefineSymbols)}: {scriptingDefineSymbols}, " +
                $"{nameof(developmentBuildFlag)}: {developmentBuildFlag}, "     +
                $"{nameof(exportAsGoogleAndroidFlag)}: {exportAsGoogleAndroidFlag}";
        }
    }
}
