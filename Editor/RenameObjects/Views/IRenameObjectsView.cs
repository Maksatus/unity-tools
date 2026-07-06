using UnityTools.Editor.RenameObjects.Data;
using System;
using System.Collections.Generic;

namespace UnityTools.Editor.RenameObjects.Views
{
    public interface IRenameObjectsView
    {
        event Action<RenameRules> RulesChanged;
        event Action ApplyRequested;

        void ShowPreview(IReadOnlyList<RenamePreviewItem> items);
    }
}
