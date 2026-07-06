using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Editor.SceneSelector
{
    public interface ISceneRepository
    {
        IReadOnlyList<SceneInfo> LoadScenes();
    }

    public class AssetDatabaseSceneRepository : ISceneRepository
    {
        private const string SceneAssetFilter = "t:Scene";
        private const string AssetsFolderPrefix = "Assets/";

        public IReadOnlyList<SceneInfo> LoadScenes()
        {
            return AssetDatabase.FindAssets(SceneAssetFilter)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.StartsWith(AssetsFolderPrefix))
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
