using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Editor.SceneSelector
{
    public class SceneSelectorWindow : EditorWindow
    {
        private TextField _searchField;
        private VisualElement _listContainer;

        [MenuItem("Tools/Scene Selector %`")]
        public static void Init()
        {
            GetWindowWithRect(typeof(SceneSelectorWindow), new Rect(0, 0, 260, 400), false, "Scene Selector");
        }

        public void CreateGUI()
        {
            _searchField = new TextField("Search:");
            _searchField.RegisterValueChangedCallback(evt => UpdateSceneList(evt.newValue));
            rootVisualElement.Add(_searchField);

            _listContainer = new ScrollView();
            _listContainer.style.height = 300;
            rootVisualElement.Add(_listContainer);

            UpdateSceneList("");
        }

        private void UpdateSceneList(string searchText)
        {
            _listContainer.Clear();
            var sceneGuids = AssetDatabase.FindAssets("t:Scene");

            var scenesByFolder = new Dictionary<string, List<string>>();

            foreach (var sceneGuid in sceneGuids)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                var folderPath = System.IO.Path.GetDirectoryName(scenePath);
                if (!scenesByFolder.ContainsKey(folderPath))
                {
                    scenesByFolder[folderPath] = new List<string>();
                }
                scenesByFolder[folderPath].Add(sceneGuid);
            }

            foreach (var folder in scenesByFolder)
            {
                var folderLabel = new Label(folder.Key);
                _listContainer.Add(folderLabel);

                foreach (var sceneGuid in folder.Value)
                {
                    var sceneButton = CreateSceneButton(sceneGuid);
                    if (sceneButton != null && sceneButton.Q<Label>().text.ToLower().Contains(searchText.ToLower()))
                    {
                        _listContainer.Add(sceneButton);
                    }
                }
            }
        }

        private static VisualElement CreateSceneButton(string sceneGuid)
        {
            var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
            var buttonGroup = new VisualElement();

            buttonGroup.style.flexDirection = FlexDirection.Row;
            buttonGroup.style.marginLeft = 3;

            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (sceneAsset == null || sceneAsset.name is "Basic" or "Standard")
            {
                return null;
            }
            var label = new Label($"{sceneAsset.name}");
            label.style.width = 200;
            buttonGroup.Add(label);

            var openButton = new Button(() =>
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            })
            {
                text = "Open"
            };

            buttonGroup.Add(openButton);
            return buttonGroup;
        }
    }
}
