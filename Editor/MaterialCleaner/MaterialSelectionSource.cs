using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.MaterialCleaner
{
    public interface IMaterialSelectionSource
    {
        IReadOnlyList<Material> GetSelectedMaterials();
    }

    public class EditorMaterialSelectionSource : IMaterialSelectionSource
    {
        public IReadOnlyList<Material> GetSelectedMaterials()
        {
            return Selection.objects.OfType<Material>().ToList();
        }
    }
}
