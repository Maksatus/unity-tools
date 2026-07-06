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
        private bool _groupByFolder = true;

        public event Action Changed;

        public SceneSelectorModel(ISceneRepository repository, bool groupByFolder = true)
        {
            _repository = repository;
            _groupByFolder = groupByFolder;
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

        public void SetGroupByFolder(bool groupByFolder)
        {
            if (groupByFolder == _groupByFolder) return;

            _groupByFolder = groupByFolder;
            Changed?.Invoke();
        }

        public IReadOnlyList<SceneGroup> GetFilteredGroups()
        {
            var filtered = _allScenes.Where(MatchesFilter);

            if (!_groupByFolder)
            {
                var flat = filtered.OrderBy(scene => scene.Name, StringComparer.OrdinalIgnoreCase).ToList();
                return flat.Count == 0
                    ? Array.Empty<SceneGroup>()
                    : new[] { new SceneGroup(string.Empty, flat) };
            }

            return filtered
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
