namespace UnityTools.Editor.SceneSelector.Data
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
}
