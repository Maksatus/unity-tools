using UnityTools.Editor.SceneSelector.Services;
using UnityTools.Editor.SceneSelector.Settings;
using UnityTools.Editor.SceneSelector.Views;
using static UnityTools.Editor.SceneSelector.SceneSelectorConstants;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityTools.Editor.SceneSelector
{
    public class SceneSelectorWindow : EditorWindow
    {
        private SceneSelectorPresenter _presenter;
        private ISceneSelectorView _view;

        [MenuItem(MenuPath)]
        public static void Init()
        {
            var window = GetWindow<SceneSelectorWindow>(WindowTitle);
            window.Show();
        }

        public void CreateGUI()
        {
            var groupByFolder = SceneSelectorUserState.instance.IsGroupedByFolder;

            _view = new SceneSelectorView(rootVisualElement, groupByFolder);
            _view.ContentChanged += UpdateWindowSize;
            _view.GroupingChanged += OnGroupingChanged;

            var model = new SceneSelectorModel(new AssetDatabaseSceneRepository(SceneSelectorSettings.instance),
                groupByFolder);
            _presenter = new SceneSelectorPresenter(model, _view, new EditorSceneOpener());
            _presenter.Start();

            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosed += OnSceneClosed;
            SceneSelectorSettings.Changed += OnSettingsChanged;
        }

        private void OnSettingsChanged() => _presenter?.RefreshScenes();

        private void OnGroupingChanged(bool groupByFolder) =>
            SceneSelectorUserState.instance.SetGroupByFolder(groupByFolder);

        private void OnProjectChange() => _presenter?.RefreshScenes();

        private void OnSceneOpened(Scene scene, OpenSceneMode mode) => _presenter?.RefreshScenes();

        private void OnSceneClosed(Scene scene) => _presenter?.RefreshScenes();

        private void OnDisable()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneClosed -= OnSceneClosed;
            SceneSelectorSettings.Changed -= OnSettingsChanged;

            if (_view != null)
            {
                _view.ContentChanged -= UpdateWindowSize;
                _view.GroupingChanged -= OnGroupingChanged;
            }

            _presenter?.Dispose();
            _presenter = null;
            _view = null;
        }

        private void UpdateWindowSize()
        {
            var width = Mathf.Clamp(_view.PreferredWidth, MinWindowWidth, MaxWindowWidth);
            var height = Mathf.Clamp(_view.PreferredHeight, MinWindowHeight, MaxWindowHeight);
            minSize = new Vector2(width, height);
            maxSize = new Vector2(width, height);
        }
    }
}
