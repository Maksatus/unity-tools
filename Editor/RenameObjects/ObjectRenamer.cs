using UnityEditor;
using UnityEngine;

namespace Editor.RenameObjects
{
    public interface IObjectRenamer
    {
        void Rename(Object target, string newName);
    }

    public class EditorObjectRenamer : IObjectRenamer
    {
        private const string UndoOperationName = "Rename Objects";

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
