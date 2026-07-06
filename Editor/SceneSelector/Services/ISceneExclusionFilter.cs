namespace UnityTools.Editor.SceneSelector.Services
{
    public interface ISceneExclusionFilter
    {
        bool IsExcluded(string scenePath);
    }
}
