using UnityTools.Editor.SceneSelector.Data;
using UnityTools.Editor.SceneSelector.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityTools.Editor.SceneSelector
{
    public class SceneSelectorModel
    {
        private readonly ISceneRepository _repository;
        private IReadOnlyList<SceneInfo> _allScenes = Array.Empty<SceneInfo>();
        private string _searchFilter = string.Empty;

        public event Action Changed;

        public SceneSelectorModel(ISceneRepository repository)
        {
            _repository = repository;
        }

        public void Refresh()
        {
            _allScenes = _repository.LoadScenes();
            Changed?.Invoke();
        }

        public void SetSearchFilter(string filter)
        {
            var normalized = filter?.Trim() ?? string.Empty;
            if (normalized == _searchFilter) return;

            _searchFilter = normalized;
            Changed?.Invoke();
        }

        public IReadOnlyList<SceneGroup> GetFilteredGroups()
        {
            return _allScenes
                .Where(MatchesFilter)
                .GroupBy(scene => scene.Folder)
                .Select(group => new SceneGroup(group.Key, group.ToList()))
                .ToList();
        }

        private bool MatchesFilter(SceneInfo scene)
        {
            return string.IsNullOrEmpty(_searchFilter) || scene.Name.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
