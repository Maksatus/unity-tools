using UnityEditor;
using UnityEngine;
using static UnityTools.Editor.SceneSelector.SceneSelectorConstants;

namespace UnityTools.Editor.SceneSelector.Settings
{
    [FilePath(UserStateFilePath, FilePathAttribute.Location.ProjectFolder)]
    public class SceneSelectorUserState : ScriptableSingleton<SceneSelectorUserState>
    {
        [SerializeField] private bool GroupByFolder = true;

        public bool IsGroupedByFolder => GroupByFolder;

        public void SetGroupByFolder(bool value)
        {
            if (GroupByFolder == value) return;

            GroupByFolder = value;
            Save(true);
        }
    }
}
