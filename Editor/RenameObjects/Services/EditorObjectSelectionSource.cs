using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Editor.RenameObjects.Services
{
    public class EditorObjectSelectionSource : IObjectSelectionSource
    {
        public IReadOnlyList<Object> GetSelectedObjects()
        {
            return Selection.objects;
        }
    }
}
