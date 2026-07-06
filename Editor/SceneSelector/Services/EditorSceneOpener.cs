using UnityTools.Editor.SceneSelector.Data;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityTools.Editor.SceneSelector.Services
{
    public class EditorSceneOpener : ISceneOpener
    {
        public void Open(SceneInfo scene)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene(scene.Path, OpenSceneMode.Single);
        }

        public void OpenAdditive(SceneInfo scene)
        {
            EditorSceneManager.OpenScene(scene.Path, OpenSceneMode.Additive);
        }

        public void CloseAdditive(SceneInfo scene)
        {
            var loadedScene = SceneManager.GetSceneByPath(scene.Path);

            if (!loadedScene.isLoaded || SceneManager.loadedSceneCount <= 1) return;
            if (loadedScene.isDirty && !EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[] { loadedScene })) return;

            EditorSceneManager.CloseScene(loadedScene, true);
        }

        public bool IsLoaded(SceneInfo scene)
        {
            return SceneManager.GetSceneByPath(scene.Path).isLoaded;
        }
    }
}
