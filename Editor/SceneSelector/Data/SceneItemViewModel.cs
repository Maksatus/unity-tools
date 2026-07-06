namespace UnityTools.Editor.SceneSelector.Data
{
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
}
