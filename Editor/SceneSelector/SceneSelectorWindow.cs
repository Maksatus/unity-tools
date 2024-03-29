using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Editor.SceneSelector
{
    public class SceneSelectorWindow : EditorWindow
    {
        private const string SearchFieldLabel = "Search:";
        private const string WindowTitle = "Scene Selector";
        private const string SceneAssetType = "t:Scene";
        private const string AssetsFolderPrefix = "Assets/";
        private const int FolderLabelFontSize = 13;
        private const int SceneLabelWidth = 200;
        private const float ButtonGroupMarginLeft = 3f;
        private const int WindowWidth = 260;
        private const int MinWindowHeight = 200;
        private const int MaxWindowHeight = 700; 

        private const int BaseItemHeight = 20; 
        private const int SearchFieldHeight = 30;
        
        private TextField _searchField;
        private ScrollView _listContainer;

        [MenuItem("Tools/Scene Selector %`")]
        public static void Init()
        {
            var window = GetWindow<SceneSelectorWindow>(WindowTitle);
            window.Show();
        }

        public void CreateGUI()
        {
            CreateSearchField();
            CreateListContainer();
            UpdateSceneList("");
        }

        private void CreateSearchField()
        {
            _searchField = new TextField(SearchFieldLabel);
            _searchField.RegisterValueChangedCallback(evt => UpdateSceneList(evt.newValue));
            rootVisualElement.Add(_searchField);
        }

        private void CreateListContainer()
        {
            _listContainer = new ScrollView { style = { height = MaxWindowHeight } };
            rootVisualElement.Add(_listContainer);
        }

        private void UpdateSceneList(string searchText)
        {
            _listContainer.Clear();
            var scenesByFolder = GetScenesOrganizedByFolder();
            DisplayScenesByFolder(scenesByFolder, searchText.ToLower());
            
            UpdateWindowSize(scenesByFolder, searchText);
        }
            
        private void UpdateWindowSize(Dictionary<string, List<string>> scenesByFolder, string searchText)
        {
            int itemCount = scenesByFolder.Sum(folder => folder.Value.Count(scene => scene.ToLower().Contains(searchText.ToLower()))) + scenesByFolder.Count; // Подсчитываем общее количество элементов, включая заголовки папок
            int totalHeight = itemCount * BaseItemHeight + SearchFieldHeight;
            
            totalHeight = Mathf.Clamp(totalHeight, MinWindowHeight, MaxWindowHeight);
            
            minSize = new Vector2(WindowWidth, totalHeight);
            maxSize = new Vector2(WindowWidth, totalHeight);
        }
        
        private static Dictionary<string, List<string>> GetScenesOrganizedByFolder()
        {
            var sceneGuids = AssetDatabase.FindAssets(SceneAssetType)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.StartsWith(AssetsFolderPrefix))
                .ToList();

            var scenesByFolder = new Dictionary<string, List<string>>();

            foreach (var scenePath in sceneGuids)
            {
                var folderPath = Path.GetDirectoryName(scenePath)?.Split(Path.DirectorySeparatorChar).LastOrDefault();
                if (folderPath == null) continue;

                if (!scenesByFolder.ContainsKey(folderPath))
                {
                    scenesByFolder[folderPath] = new List<string>();
                }
                scenesByFolder[folderPath].Add(scenePath);
            }

            return scenesByFolder;
        }

        private void DisplayScenesByFolder(Dictionary<string, List<string>> scenesByFolder, string searchText)
        {
            foreach (var folder in scenesByFolder)
            {
                var folderLabel = new Label(folder.Key)
                {
                    style =
                    {
                        color = new StyleColor(Color.white),
                        fontSize = FolderLabelFontSize
                    }
                };
                _listContainer.Add(folderLabel);

                foreach (var scenePath in folder.Value)
                {
                    var sceneButton = CreateSceneButton(scenePath);
                    if (sceneButton != null && sceneButton.Q<Label>().text.ToLower().Contains(searchText))
                    {
                        _listContainer.Add(sceneButton);
                    }
                }
            }
        }

        private static VisualElement CreateSceneButton(string scenePath)
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (sceneAsset == null) return null;

            var buttonGroup = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    marginLeft = ButtonGroupMarginLeft
                }
            };

            var label = new Label(sceneAsset.name) { style = { width = SceneLabelWidth } };
            buttonGroup.Add(label);

            var openButton = new Button(() => EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single))
            {
                text = "Open"
            };
            buttonGroup.Add(openButton);

            return buttonGroup;
        }
    }
}
