using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.MaterialCleaner
{
    public class MaterialCleanerModel
    {
        private readonly IMaterialSelectionSource _selectionSource;

        public IReadOnlyList<Material> Materials { get; private set; } = Array.Empty<Material>();

        public event Action Changed;

        public MaterialCleanerModel(IMaterialSelectionSource selectionSource)
        {
            _selectionSource = selectionSource;
        }

        public void Refresh()
        {
            Materials = _selectionSource.GetSelectedMaterials();
            Changed?.Invoke();
        }
    }
}
