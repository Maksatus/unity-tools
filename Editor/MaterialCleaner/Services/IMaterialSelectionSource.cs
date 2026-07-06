using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Editor.MaterialCleaner.Services
{
    public interface IMaterialSelectionSource
    {
        IReadOnlyList<Material> GetSelectedMaterials();
    }
}
