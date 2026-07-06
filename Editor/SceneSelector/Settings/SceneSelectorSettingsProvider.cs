using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityTools.Editor.SceneSelector.SceneSelectorConstants;

namespace UnityTools.Editor.SceneSelector.Settings
{
    public static class SceneSelectorSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider Create()
        {
            return new SettingsProvider(SettingsPath, SettingsScope.Project)
            {
                label = "Scene Selector",
                keywords = new HashSet<string> { "scene", "selector", "exclude", "ignore", "folder" },
                guiHandler = _ => DrawSettings()
            };
        }

        private static void DrawSettings()
        {
            var settings = SceneSelectorSettings.instance;
            settings.hideFlags &= ~HideFlags.NotEditable;

            var serializedObject = new SerializedObject(settings);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Scenes matching these lists are hidden from the Scene Selector window.\n" +
                "Folders — path prefix, e.g. 'Assets/ThirdParty'.\n" +
                "Scenes — scene name ('Boot') or full path ('Assets/Scenes/Boot.unity').",
                MessageType.Info);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_excludedFolders"),
                new GUIContent("Excluded Folders"),
                includeChildren: true);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_excludedScenes"),
                new GUIContent("Excluded Scenes"),
                includeChildren: true);

            if (serializedObject.ApplyModifiedProperties())
                settings.SaveAndNotify();
        }
    }
}
