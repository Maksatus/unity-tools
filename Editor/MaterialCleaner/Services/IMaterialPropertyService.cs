using UnityEngine;
using UnityTools.Editor.MaterialCleaner.Data;

namespace UnityTools.Editor.MaterialCleaner.Services
{
    public interface IMaterialPropertyService
    {
        MaterialInspection Inspect(Material material);
        void RemoveProperty(Material material, MaterialSavedPropertyType type, string propertyName);
        int RemoveUnusedProperties(Material material, MaterialSavedPropertyType type);
        int TransferFloatsToInts(Material material);
    }
}
