namespace Editor.RenameObjects
{
    public class RenameObjectsPresenter
    {
        private readonly RenameObjectsModel _model;
        private readonly IRenameObjectsView _view;
        private readonly IObjectRenamer _renamer;

        public RenameObjectsPresenter(RenameObjectsModel model, IRenameObjectsView view, IObjectRenamer renamer)
        {
            _model = model;
            _view = view;
            _renamer = renamer;

            _model.Changed += OnModelChanged;
            _view.RulesChanged += _model.SetRules;
            _view.ApplyRequested += OnApplyRequested;
        }

        public void Start() => _model.RefreshSelection();

        public void RefreshSelection() => _model.RefreshSelection();

        public void Dispose()
        {
            _model.Changed -= OnModelChanged;
            _view.RulesChanged -= _model.SetRules;
            _view.ApplyRequested -= OnApplyRequested;
        }

        private void OnModelChanged() => _view.ShowPreview(_model.GetPreview());

        private void OnApplyRequested()
        {
            foreach (var item in _model.GetPreview())
            {
                if (item.IsChanged)
                    _renamer.Rename(item.Target, item.NewName);
            }

            _model.RefreshSelection();
        }
    }
}
