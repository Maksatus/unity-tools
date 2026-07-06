namespace UnityTools.Editor.SceneSelector
{
    public static class SceneSelectorConstants
    {
        public const string MenuPath = "Tools/Scene Selector %`";
        public const string WindowTitle = "Scene Selector";
        public const float MinWindowWidth = 260f;
        public const float MaxWindowWidth = 600f;
        public const float MinWindowHeight = 200f;
        public const float MaxWindowHeight = 700f;

        public const string StyleSheetPath = "Packages/com.maksatus.unity-tools/Editor/SceneSelector/Views/SceneSelectorView.uss";
        public const string SceneIconName = "SceneAsset Icon";
        public const string FolderIconName = "Folder Icon";
        public const string GroupToggleText = "≡";
        public const string ShowFlatTooltip = "Show flat list";
        public const string ShowGroupedTooltip = "Group by folder";
        public const string AddAdditiveText = "+";
        public const string RemoveAdditiveText = "−";
        public const string AddAdditiveTooltip = "Load scene additively";
        public const string RemoveAdditiveTooltip = "Remove scene from hierarchy";
        public const float RowHeight = 26f;
        public const float RowWidthOverhead = 130f;
        public const float FolderHeaderHeight = 26f;
        public const float FolderHeaderWidthOverhead = 80f;
        public const float HeaderHeight = 34f;
        public const float EmptyStateHeight = 80f;

        public const string SceneAssetFilter = "t:Scene";
        public const string AssetsFolderPrefix = "Assets/";

        public const string SettingsPath = "Project/Unity Tools/Scene Selector";
        public const string SettingsFilePath = "ProjectSettings/SceneSelectorSettings.asset";
        public const string UserStateFilePath = "UserSettings/SceneSelectorState.asset";
    }
}
