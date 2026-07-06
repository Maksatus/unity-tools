using UnityTools.Editor.SceneSelector.Services;
using static UnityTools.Editor.SceneSelector.SceneSelectorConstants;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Editor.SceneSelector.Settings
{
    [FilePath(SettingsFilePath, FilePathAttribute.Location.ProjectFolder)]
    public class SceneSelectorSettings : ScriptableSingleton<SceneSelectorSettings>, ISceneExclusionFilter
    {
        [SerializeField] private List<string> ExcludedFolders = new();
        [SerializeField] private List<string> ExcludedScenes = new();

        public static event Action Changed;

        public bool IsExcluded(string scenePath)
        {
            var normalizedPath = NormalizePath(scenePath);

            foreach (var folder in ExcludedFolders)
            {
                if (string.IsNullOrWhiteSpace(folder))
                    continue;

                var normalizedFolder = NormalizePath(folder).TrimEnd('/');
                if (normalizedPath.StartsWith(normalizedFolder + "/", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            var sceneName = Path.GetFileNameWithoutExtension(scenePath);

            foreach (var scene in ExcludedScenes)
            {
                if (string.IsNullOrWhiteSpace(scene)) continue;

                var isPath = scene.Contains('/') || scene.Contains('\\');
                var matches = isPath
                    ? string.Equals(NormalizePath(scene), normalizedPath, StringComparison.OrdinalIgnoreCase)
                    : string.Equals(scene.Trim(), sceneName, StringComparison.OrdinalIgnoreCase);

                if (matches) return true;
            }

            return false;
        }

        public void SaveAndNotify()
        {
            Save(true);
            Changed?.Invoke();
        }

        private static string NormalizePath(string path)
        {
            return path.Trim().Replace('\\', '/');
        }
    }
}
