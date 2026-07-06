using UnityTools.Editor.SceneSelector.Data;
using System.Collections.Generic;

namespace UnityTools.Editor.SceneSelector.Services
{
    public interface ISceneRepository
    {
        IReadOnlyList<SceneInfo> LoadScenes();
    }
}
