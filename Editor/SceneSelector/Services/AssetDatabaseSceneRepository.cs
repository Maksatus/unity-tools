using UnityTools.Editor.SceneSelector.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using static UnityTools.Editor.SceneSelector.SceneSelectorConstants;

namespace UnityTools.Editor.SceneSelector.Services
{
    public class AssetDatabaseSceneRepository : ISceneRepository
    {
        private readonly ISceneExclusionFilter _exclusionFilter;

        public AssetDatabaseSceneRepository(ISceneExclusionFilter exclusionFilter)
        {
            _exclusionFilter = exclusionFilter;
        }

        public IReadOnlyList<SceneInfo> LoadScenes()
        {
            return AssetDatabase.FindAssets(SceneAssetFilter)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.StartsWith(AssetsFolderPrefix))
                .Where(path => !_exclusionFilter.IsExcluded(path))
                .Select(CreateSceneInfo)
                .ToList();
        }

        private static SceneInfo CreateSceneInfo(string path)
        {
            var folder = Path.GetDirectoryName(path)?
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .LastOrDefault() ?? string.Empty;

            return new SceneInfo(Path.GetFileNameWithoutExtension(path), path, folder);
        }
    }
}
