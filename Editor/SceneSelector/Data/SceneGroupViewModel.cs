using System.Collections.Generic;

namespace UnityTools.Editor.SceneSelector.Data
{
    public readonly struct SceneGroupViewModel
    {
        public string Folder { get; }
        public IReadOnlyList<SceneItemViewModel> Scenes { get; }

        public SceneGroupViewModel(string folder, IReadOnlyList<SceneItemViewModel> scenes)
        {
            Folder = folder;
            Scenes = scenes;
        }
    }
}
