using UnityTools.Editor.SceneSelector.Data;
namespace UnityTools.Editor.SceneSelector.Services
{
    public interface ISceneOpener
    {
        void Open(SceneInfo scene);
        void OpenAdditive(SceneInfo scene);
        void CloseAdditive(SceneInfo scene);
        bool IsLoaded(SceneInfo scene);
    }
}
