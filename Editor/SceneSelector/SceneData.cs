using System.Collections.Generic;

namespace Editor.SceneSelector
{
    public readonly struct SceneInfo
    {
        public string Name { get; }
        public string Path { get; }
        public string Folder { get; }

        public SceneInfo(string name, string path, string folder)
        {
            Name = name;
            Path = path;
            Folder = folder;
        }
    }

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

    public readonly struct SceneItemViewModel
    {
        public SceneInfo Scene { get; }
        public bool IsLoaded { get; }

        public SceneItemViewModel(SceneInfo scene, bool isLoaded)
        {
            Scene = scene;
            IsLoaded = isLoaded;
        }
    }

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
