using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.SceneSelector
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

    public class SceneSelectorView : ISceneSelectorView
    {
        private const string StyleSheetPath =
            "Packages/com.maksatus.unity-tools/Editor/SceneSelector/SceneSelectorView.uss";

        private const string SceneIconName = "SceneAsset Icon";
        private const string FolderIconName = "Folder Icon";
        private const string EmptyIconName = "d_SearchWindowIcon";

        private const string AddAdditiveText = "+";
        private const string RemoveAdditiveText = "−";
        private const string AddAdditiveTooltip = "Load scene additively";
        private const string RemoveAdditiveTooltip = "Remove scene from hierarchy";

        private const float RowHeight = 26f;
        private const float FolderHeaderHeight = 26f;
        private const float HeaderHeight = 34f;
        private const float EmptyStateHeight = 80f;

        private readonly ToolbarSearchField _searchField;
        private readonly ScrollView _sceneList;
        private float _contentHeight;

        public event Action<string> SearchChanged;
        public event Action<SceneInfo> OpenSceneRequested;
        public event Action<SceneInfo> AdditiveToggleRequested;
        public event Action ContentChanged;

        public float PreferredHeight => _contentHeight + HeaderHeight;

        public SceneSelectorView(VisualElement root)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(StyleSheetPath);
            if (styleSheet != null)
                root.styleSheets.Add(styleSheet);

            var header = new VisualElement();
            header.AddToClassList("scene-selector__header");
            root.Add(header);

            _searchField = new ToolbarSearchField();
            _searchField.AddToClassList("scene-selector__search");
            _searchField.RegisterValueChangedCallback(evt => SearchChanged?.Invoke(evt.newValue));
            header.Add(_searchField);

            _sceneList = new ScrollView();
            _sceneList.AddToClassList("scene-selector__list");
            root.Add(_sceneList);
        }

        public void ShowScenes(IReadOnlyList<SceneGroupViewModel> groups)
        {
            _sceneList.Clear();

            if (groups.Count == 0)
            {
                _sceneList.Add(CreateEmptyState());
                _contentHeight = EmptyStateHeight;
            }
            else
            {
                foreach (var group in groups)
                {
                    _sceneList.Add(CreateFolderHeader(group));

                    foreach (var item in group.Scenes)
                        _sceneList.Add(CreateSceneRow(item));
                }

                _contentHeight = groups.Count * FolderHeaderHeight
                    + groups.Sum(group => group.Scenes.Count) * RowHeight;
            }

            ContentChanged?.Invoke();
        }

        private static VisualElement CreateFolderHeader(SceneGroupViewModel group)
        {
            var header = new VisualElement();
            header.AddToClassList("scene-selector__folder-header");

            header.Add(CreateIcon(FolderIconName, "scene-selector__folder-icon"));

            var nameLabel = new Label(group.Folder);
            nameLabel.AddToClassList("scene-selector__folder-name");
            header.Add(nameLabel);

            var countLabel = new Label(group.Scenes.Count.ToString());
            countLabel.AddToClassList("scene-selector__folder-count");
            header.Add(countLabel);

            return header;
        }

        private static VisualElement CreateEmptyState()
        {
            var container = new VisualElement();
            container.AddToClassList("scene-selector__empty");

            container.Add(CreateIcon(EmptyIconName, "scene-selector__empty-icon"));

            var label = new Label("No scenes found");
            label.AddToClassList("scene-selector__empty-label");
            container.Add(label);

            return container;
        }

        private VisualElement CreateSceneRow(SceneItemViewModel item)
        {
            var scene = item.Scene;

            var row = new Button(() => OpenSceneRequested?.Invoke(scene)) { text = string.Empty };
            row.AddToClassList("scene-selector__row");
            if (item.IsLoaded)
                row.AddToClassList("scene-selector__row--loaded");
            row.tooltip = scene.Path;

            row.Add(CreateIcon(SceneIconName, "scene-selector__scene-icon"));

            var nameLabel = new Label(scene.Name);
            nameLabel.AddToClassList("scene-selector__scene-name");
            row.Add(nameLabel);

            var openHint = new Label("Open");
            openHint.AddToClassList("scene-selector__open-hint");
            row.Add(openHint);

            row.Add(CreateAdditiveButton(item));

            return row;
        }

        private Button CreateAdditiveButton(SceneItemViewModel item)
        {
            var button = new Button(() => AdditiveToggleRequested?.Invoke(item.Scene))
            {
                text = item.IsLoaded ? RemoveAdditiveText : AddAdditiveText,
                tooltip = item.IsLoaded ? RemoveAdditiveTooltip : AddAdditiveTooltip
            };
            button.AddToClassList("scene-selector__additive-button");
            if (item.IsLoaded)
                button.AddToClassList("scene-selector__additive-button--loaded");
            
            button.RegisterCallback<PointerDownEvent>(evt => evt.StopPropagation());
            button.RegisterCallback<PointerUpEvent>(evt => evt.StopPropagation());

            return button;
        }

        private static Image CreateIcon(string iconName, string className)
        {
            var icon = new Image
            {
                image = EditorGUIUtility.IconContent(iconName).image,
                scaleMode = UnityEngine.ScaleMode.ScaleToFit
            };
            icon.AddToClassList(className);
            return icon;
        }
    }
}
