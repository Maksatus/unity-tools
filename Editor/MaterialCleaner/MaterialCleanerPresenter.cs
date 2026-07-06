using System.Linq;
using UnityEngine;
using UnityTools.Editor.MaterialCleaner.Data;
using UnityTools.Editor.MaterialCleaner.Services;
using UnityTools.Editor.MaterialCleaner.Views;

namespace UnityTools.Editor.MaterialCleaner
{
    public class MaterialCleanerPresenter
    {
        private readonly MaterialCleanerModel _model;
        private readonly IMaterialCleanerView _view;
        private readonly IMaterialPropertyService _propertyService;

        public MaterialCleanerPresenter(MaterialCleanerModel model, IMaterialCleanerView view, IMaterialPropertyService propertyService)
        {
            _model = model;
            _view = view;
            _propertyService = propertyService;

            _model.Changed += OnModelChanged;
            _view.RemovePropertyRequested += OnRemovePropertyRequested;
            _view.RemoveUnusedOfTypeRequested += OnRemoveUnusedOfTypeRequested;
            _view.RemoveAllUnusedRequested += OnRemoveAllUnusedRequested;
            _view.TransferFloatsRequested += OnTransferFloatsRequested;
        }

        public void Start() => _model.Refresh();

        public void RefreshMaterials() => _model.Refresh();

        public void Dispose()
        {
            _model.Changed -= OnModelChanged;
            _view.RemovePropertyRequested -= OnRemovePropertyRequested;
            _view.RemoveUnusedOfTypeRequested -= OnRemoveUnusedOfTypeRequested;
            _view.RemoveAllUnusedRequested -= OnRemoveAllUnusedRequested;
            _view.TransferFloatsRequested -= OnTransferFloatsRequested;
        }

        private void OnModelChanged()
        {
            _view.ShowMaterials(_model.Materials.Select(_propertyService.Inspect).ToList());
        }

        private void OnRemovePropertyRequested(Material material, MaterialSavedPropertyType type, string propertyName)
        {
            _propertyService.RemoveProperty(material, type, propertyName);
            OnModelChanged();
        }

        private void OnRemoveUnusedOfTypeRequested(MaterialSavedPropertyType type)
        {
            foreach (var material in _model.Materials)
                _propertyService.RemoveUnusedProperties(material, type);

            OnModelChanged();
        }

        private void OnRemoveAllUnusedRequested()
        {
            foreach (var material in _model.Materials)
            foreach (MaterialSavedPropertyType type in System.Enum.GetValues(typeof(MaterialSavedPropertyType)))
                _propertyService.RemoveUnusedProperties(material, type);

            OnModelChanged();
        }

        private void OnTransferFloatsRequested()
        {
            foreach (var material in _model.Materials)
                _propertyService.TransferFloatsToInts(material);

            OnModelChanged();
        }
    }
}
