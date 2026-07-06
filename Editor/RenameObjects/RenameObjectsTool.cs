using UnityTools.Editor.RenameObjects.Services;
using UnityTools.Editor.RenameObjects.Views;
using UnityEditor;
using UnityEngine;
using static UnityTools.Editor.RenameObjects.RenameObjectsConstants;

namespace UnityTools.Editor.RenameObjects
{
    public class RenameObjectsTool : EditorWindow
    {
        private RenameObjectsPresenter _presenter;

        [MenuItem(MenuPath)]
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
