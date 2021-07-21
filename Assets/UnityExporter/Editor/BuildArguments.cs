using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityExporter
{
    /// <summary>
    ///     Filters all arguments relevant for building and exporting from <see cref="Environment.GetEnvironmentVariables()" />
    ///     .
    /// </summary>
    public class BuildArguments
    {
        public const string k_Batchmode   = "batchmode";
        public const string k_ExportPath  = "exportPath";
        public const string k_Version     = "newVersion";
        public const string k_VersionCode = "newVersionCode";

        private Dictionary<string, string> m_Arguments = new Dictionary<string, string>();
        private string[]                   m_BuildArguments;

        private bool isBatchmode { get; }

        public BuildArguments()
        {
            m_BuildArguments = Environment.GetCommandLineArgs();
            Debug.Log($"CommandlineArgs: '{m_BuildArguments.ElementsToString()}'");

            // mapping to dictionary
            string[] identifiers = { k_ExportPath, k_Version, k_VersionCode };
            for (int i = 0; i < m_BuildArguments.Length; i++)
            {
                foreach (string identifier in identifiers)
                {
                    if (m_BuildArguments[i].Contains(identifier) && !m_Arguments.ContainsKey(identifier))
                    {
                        m_Arguments.Add(identifier, m_BuildArguments[++i]);
                    }
                }

                if (m_BuildArguments[i].Contains(k_Batchmode))
                {
                    isBatchmode = true;
                }
            }

            if (m_Arguments.ContainsKey(k_ExportPath))
            {
                m_Arguments[k_ExportPath] = Path.GetFullPath(m_Arguments[k_ExportPath]);
            }

            // providing fallback for easier in-editor development of the package
            if (!isBatchmode)
            {
                if (!m_Arguments.ContainsKey(k_ExportPath))
                    m_Arguments.Add(k_ExportPath, string.Empty);

                if (string.IsNullOrEmpty(m_Arguments[k_ExportPath]))
                    m_Arguments[k_ExportPath] = Path.Combine(
                        Application.dataPath, "..",
                        "Builds",
                        EditorUserBuildSettings.activeBuildTarget + "-export");
            }
        }

        public string this[string key] => m_Arguments.TryGetValue(key, out string value) ? value : string.Empty;

        public override string ToString()
        {
            return m_Arguments.ElementsToString();
        }
    }
}
