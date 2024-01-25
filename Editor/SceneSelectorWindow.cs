using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class SceneSelectorWindow : EditorWindow
    {
        private TextField _searchField;
        private VisualElement _listContainer;

        [MenuItem("Tools/Scene Selector %`")]
        public static void Init()
        {
            GetWindowWithRect(typeof(SceneSelectorWindow), new Rect(0, 0, 230, 350), false, "Scene Selector");
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

            var saveSceneButton = new Button(() =>
            {
                Debug.Log("Save");
                AssetDatabase.SaveAssets();
                EditorSceneManager.SaveOpenScenes();
            })
            {
                text = "Save scene!"
            };
            rootVisualElement.Add(saveSceneButton);
        }

        private void UpdateSceneList(string searchText)
        {
            _listContainer.Clear();
            var sceneGuids = AssetDatabase.FindAssets("t:Scene");

            foreach (var sceneGuid in sceneGuids)
            {
                var sceneButton = CreateSceneButton(sceneGuid);
                if (sceneButton != null && sceneButton.Q<Label>().text.ToLower().Contains(searchText.ToLower()))
                {
                    _listContainer.Add(sceneButton);
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
            label.style.width = 170;
            buttonGroup.Add(label);
            
            var openButton = new Button(() =>
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single); })
            {
                text = "Open"
            };
            
            buttonGroup.Add(openButton);
            return buttonGroup;
        }
    }
}
