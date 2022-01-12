using UnityEditor;

namespace armet.BuildExporter
{
    /// <summary>
    ///     Saves settings of the Editor user in <see cref="EditorUserBuildSettings" /> and <see cref="PlayerSettings" />
    ///     before starting to build.
    ///     This way these settings can be recovered after the build is complete.
    /// </summary>
    internal class EditorUserSettings
    {
        public EditorUserSettings()
        {
            buildTarget      = EditorUserBuildSettings.activeBuildTarget;
            buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            scriptingDefineSymbols =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup);

            developmentBuildFlag      = EditorUserBuildSettings.development;
            exportAsGoogleAndroidFlag = EditorUserBuildSettings.exportAsGoogleAndroidProject;
            buildAppBundle            = EditorUserBuildSettings.buildAppBundle;

            scriptingBackend = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);
        }

        public BuildTarget             buildTarget               { get; private set; }
        public BuildTargetGroup        buildTargetGroup          { get; private set; }
        public string                  scriptingDefineSymbols    { get; private set; }
        public bool                    developmentBuildFlag      { get; private set; }
        public bool                    exportAsGoogleAndroidFlag { get; private set; }
        public bool                    buildAppBundle            { get; private set; }
        public ScriptingImplementation scriptingBackend          { get; private set; }

        public void Apply()
        {
            EditorUserBuildSettings.development                  = developmentBuildFlag;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = exportAsGoogleAndroidFlag;
            EditorUserBuildSettings.buildAppBundle               = buildAppBundle;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, scriptingDefineSymbols);
            PlayerSettings.SetScriptingBackend(buildTargetGroup, scriptingBackend);
        }

        public override string ToString()
        {
            return
                $"{nameof(buildTarget)}: {buildTarget}, "                             +
                $"{nameof(buildTargetGroup)}: {buildTargetGroup}, "                   +
                $"{nameof(scriptingDefineSymbols)}: {scriptingDefineSymbols}, "       +
                $"{nameof(developmentBuildFlag)}: {developmentBuildFlag}, "           +
                $"{nameof(exportAsGoogleAndroidFlag)}: {exportAsGoogleAndroidFlag}, " +
                $"{nameof(buildAppBundle)}: {buildAppBundle}, "                       +
                $"{nameof(scriptingBackend)}: {scriptingBackend}";
        }
    }
}
