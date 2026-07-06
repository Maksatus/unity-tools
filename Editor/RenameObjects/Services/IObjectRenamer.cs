using UnityEngine;

namespace UnityTools.Editor.RenameObjects.Services
{
    public interface IObjectRenamer
    {
        void Rename(Object target, string newName);
    }
}
