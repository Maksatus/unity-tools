using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.SceneSelector
{
    public class SceneSelectorWindow : EditorWindow
    {
        private const string WindowTitle = "Scene Selector";
        private const float WindowWidth = 260f;
        private const float MinWindowHeight = 200f;
        private const float MaxWindowHeight = 700f;

        private SceneSelectorPresenter _presenter;
        private ISceneSelectorView _view;

        [MenuItem("Tools/Scene Selector %`")]
        public static void Init()
        {
            var window = GetWindow<SceneSelectorWindow>(WindowTitle);
            window.Show();
        }

        public void CreateGUI()
        {
            _view = new SceneSelectorView(rootVisualElement);
            _view.ContentChanged += UpdateWindowSize;

            var model = new SceneSelectorModel(new AssetDatabaseSceneRepository());
            _presenter = new SceneSelectorPresenter(model, _view, new EditorSceneOpener());
            _presenter.Start();

            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosed += OnSceneClosed;
        }

        private void OnProjectChange() => _presenter?.RefreshScenes();

        private void OnSceneOpened(Scene scene, OpenSceneMode mode) => _presenter?.RefreshScenes();

        private void OnSceneClosed(Scene scene) => _presenter?.RefreshScenes();

        private void OnDisable()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneClosed -= OnSceneClosed;

            if (_view != null)
                _view.ContentChanged -= UpdateWindowSize;

            _presenter?.Dispose();
            _presenter = null;
            _view = null;
        }

        private void UpdateWindowSize()
        {
            var height = Mathf.Clamp(_view.PreferredHeight, MinWindowHeight, MaxWindowHeight);
            minSize = new Vector2(WindowWidth, height);
            maxSize = new Vector2(WindowWidth, height);
        }
    }
}
