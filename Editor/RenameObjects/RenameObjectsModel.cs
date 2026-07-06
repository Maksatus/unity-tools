using UnityTools.Editor.RenameObjects.Data;
using UnityTools.Editor.RenameObjects.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityTools.Editor.RenameObjects
{
    public class RenameObjectsModel
    {
        private readonly IObjectSelectionSource _selectionSource;
        private readonly IRenameService _renameService;
        private IReadOnlyList<Object> _objects = Array.Empty<Object>();

        public RenameRules Rules { get; private set; }

        public event Action Changed;

        public RenameObjectsModel(IObjectSelectionSource selectionSource, IRenameService renameService)
        {
            _selectionSource = selectionSource;
            _renameService = renameService;
        }

        public void RefreshSelection()
        {
            _objects = _selectionSource.GetSelectedObjects();
            Changed?.Invoke();
        }

        public void SetRules(RenameRules rules)
        {
            Rules = rules;
            Changed?.Invoke();
        }

        public IReadOnlyList<RenamePreviewItem> GetPreview()
        {
            return _objects
                .Where(target => target != null)
                .Select(target => new RenamePreviewItem(target, target.name, _renameService.Apply(target.name, Rules)))
                .ToList();
        }
    }
}
