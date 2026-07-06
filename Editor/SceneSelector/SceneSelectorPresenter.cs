using UnityTools.Editor.SceneSelector.Data;
using UnityTools.Editor.SceneSelector.Services;
using UnityTools.Editor.SceneSelector.Views;
using System.Collections.Generic;
using System.Linq;

namespace UnityTools.Editor.SceneSelector
{
    public class SceneSelectorPresenter
    {
        private readonly SceneSelectorModel _model;
        private readonly ISceneSelectorView _view;
        private readonly ISceneOpener _sceneOpener;

        public SceneSelectorPresenter(SceneSelectorModel model, ISceneSelectorView view, ISceneOpener sceneOpener)
        {
            _model = model;
            _view = view;
            _sceneOpener = sceneOpener;

            _model.Changed += OnModelChanged;
            _view.SearchChanged += _model.SetSearchFilter;
            _view.OpenSceneRequested += _sceneOpener.Open;
            _view.AdditiveToggleRequested += OnAdditiveToggleRequested;
        }

        public void Start() => _model.Refresh();

        public void RefreshScenes() => _model.Refresh();

        public void Dispose()
        {
            _model.Changed -= OnModelChanged;
            _view.SearchChanged -= _model.SetSearchFilter;
            _view.OpenSceneRequested -= _sceneOpener.Open;
            _view.AdditiveToggleRequested -= OnAdditiveToggleRequested;
        }

        private void OnModelChanged() => _view.ShowScenes(BuildViewModels(_model.GetFilteredGroups()));

        private void OnAdditiveToggleRequested(SceneInfo scene)
        {
            if (_sceneOpener.IsLoaded(scene))
                _sceneOpener.CloseAdditive(scene);
            else
                _sceneOpener.OpenAdditive(scene);

            OnModelChanged();
        }

        private IReadOnlyList<SceneGroupViewModel> BuildViewModels(IReadOnlyList<SceneGroup> groups)
        {
            return groups
                .Select(group => new SceneGroupViewModel(
                    group.Folder,
                    group.Scenes
                        .Select(scene => new SceneItemViewModel(scene, _sceneOpener.IsLoaded(scene)))
                        .ToList()))
                .ToList();
        }
    }
}
