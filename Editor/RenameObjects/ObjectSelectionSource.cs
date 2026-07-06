using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.RenameObjects
{
    public interface IObjectSelectionSource
    {
        IReadOnlyList<Object> GetSelectedObjects();
    }

    public class EditorObjectSelectionSource : IObjectSelectionSource
    {
        public IReadOnlyList<Object> GetSelectedObjects()
        {
            return Selection.objects;
        }
    }
}
