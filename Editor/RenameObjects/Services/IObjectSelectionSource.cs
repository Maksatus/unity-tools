using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Editor.RenameObjects.Services
{
    public interface IObjectSelectionSource
    {
        IReadOnlyList<Object> GetSelectedObjects();
    }
}
