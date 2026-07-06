using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Editor.MaterialCleaner.Services
{
    public class EditorMaterialSelectionSource : IMaterialSelectionSource
    {
        public IReadOnlyList<Material> GetSelectedMaterials()
        {
            return Selection.objects.OfType<Material>().ToList();
        }
    }
}
