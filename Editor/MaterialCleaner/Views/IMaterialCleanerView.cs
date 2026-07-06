using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTools.Editor.MaterialCleaner.Data;

namespace UnityTools.Editor.MaterialCleaner.Views
{
    public interface IMaterialCleanerView
    {
        event Action<Material, MaterialSavedPropertyType, string> RemovePropertyRequested;
        event Action<MaterialSavedPropertyType> RemoveUnusedOfTypeRequested;
        event Action RemoveAllUnusedRequested;
        event Action TransferFloatsRequested;

        void ShowMaterials(IReadOnlyList<MaterialInspection> materials);
    }
}
