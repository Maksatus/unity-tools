using UnityEditor;
using UnityEngine;

namespace Editor.RenameObjects
{
    public class RenameObjectsTool : EditorWindow
    {
        private const string WindowTitle = "Rename Objects";
        private const float MinWindowWidth = 300f;
        private const float MinWindowHeight = 400f;

        private RenameObjectsPresenter _presenter;

        [MenuItem("Tools/RenameObjectsTool #`")]
        public static void ShowWindow()
        {
            var window = GetWindow<RenameObjectsTool>(WindowTitle);
            window.minSize = new Vector2(MinWindowWidth, MinWindowHeight);
            window.Show();
        }

        public void CreateGUI()
        {
            var view = new RenameObjectsView(rootVisualElement);
            var model = new RenameObjectsModel(new EditorObjectSelectionSource(), new RenameService());
            _presenter = new RenameObjectsPresenter(model, view, new EditorObjectRenamer());
            _presenter.Start();
        }

        private void OnSelectionChange() => _presenter?.RefreshSelection();

        private void OnProjectChange() => _presenter?.RefreshSelection();

        private void OnDisable()
        {
            _presenter?.Dispose();
            _presenter = null;
        }
    }
}
