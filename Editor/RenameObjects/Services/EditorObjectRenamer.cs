using UnityEditor;
using UnityEngine;
using static UnityTools.Editor.RenameObjects.RenameObjectsConstants;

namespace UnityTools.Editor.RenameObjects.Services
{
    public class EditorObjectRenamer : IObjectRenamer
    {
        public void Rename(Object target, string newName)
        {
            var assetPath = AssetDatabase.GetAssetPath(target);

            if (string.IsNullOrEmpty(assetPath))
            {
                Undo.RecordObject(target, UndoOperationName);
                target.name = newName;
                return;
            }

            var error = AssetDatabase.RenameAsset(assetPath, newName);
            if (!string.IsNullOrEmpty(error))
                Debug.LogError($"Failed to rename asset '{assetPath}' to '{newName}': {error}");
        }
    }
}
