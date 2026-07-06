using UnityEditor;
using UnityEngine;

namespace Editor.MaterialCleaner
{
    public class MaterialPropertyCleaner : EditorWindow
    {
        private const string WindowTitle = "Property Cleaner";
        private const float MinWindowWidth = 380f;
        private const float MinWindowHeight = 300f;

        private MaterialCleanerPresenter _presenter;

        [MenuItem("Tools/Material Property Cleaner")]
        public static void Init()
        {
            var window = GetWindow<MaterialPropertyCleaner>(WindowTitle);
            window.minSize = new Vector2(MinWindowWidth, MinWindowHeight);
            window.Show();
        }

        public void CreateGUI()
        {
            var view = new MaterialCleanerView(rootVisualElement);
            var model = new MaterialCleanerModel(new EditorMaterialSelectionSource());
            _presenter = new MaterialCleanerPresenter(model, view, new MaterialPropertyService());
            _presenter.Start();

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnSelectionChange() => _presenter?.RefreshMaterials();

        private void OnProjectChange() => _presenter?.RefreshMaterials();

        private void OnUndoRedo() => _presenter?.RefreshMaterials();

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;

            _presenter?.Dispose();
            _presenter = null;
        }
    }
}
