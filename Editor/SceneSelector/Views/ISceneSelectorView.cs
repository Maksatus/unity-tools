using UnityTools.Editor.SceneSelector.Data;
using System;
using System.Collections.Generic;

namespace UnityTools.Editor.SceneSelector.Views
{
    public interface ISceneSelectorView
    {
        event Action<string> SearchChanged;
        event Action<SceneInfo> OpenSceneRequested;
        event Action<SceneInfo> AdditiveToggleRequested;
        event Action ContentChanged;

        float PreferredHeight { get; }

        void ShowScenes(IReadOnlyList<SceneGroupViewModel> groups);
    }
}
