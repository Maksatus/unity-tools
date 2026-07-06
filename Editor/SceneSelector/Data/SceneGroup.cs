using System.Collections.Generic;

namespace UnityTools.Editor.SceneSelector.Data
{
    public readonly struct SceneGroup
    {
        public string Folder { get; }
        public IReadOnlyList<SceneInfo> Scenes { get; }

        public SceneGroup(string folder, IReadOnlyList<SceneInfo> scenes)
        {
            Folder = folder;
            Scenes = scenes;
        }
    }
}
