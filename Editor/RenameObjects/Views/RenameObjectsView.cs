using UnityTools.Editor.RenameObjects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityTools.Editor.RenameObjects.RenameObjectsConstants;

namespace UnityTools.Editor.RenameObjects.Views
{
    public class RenameObjectsView : IRenameObjectsView
    {
        private readonly TextField _prefixField;
        private readonly TextField _suffixField;
        private readonly TextField _replaceField;
        private readonly TextField _replaceByField;
        private readonly TextField _insertBeforeAnchorField;
        private readonly TextField _insertBeforeTextField;
        private readonly TextField _insertAfterAnchorField;
        private readonly TextField _insertAfterTextField;
        private readonly Toggle _lowercaseToggle;
        private readonly Toggle _trimLastWordToggle;
        private readonly Button _applyButton;
        private readonly ScrollView _previewList;

        public event Action<RenameRules> RulesChanged;
        public event Action ApplyRequested;

        public RenameObjectsView(VisualElement root)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(StyleSheetPath);
            if (styleSheet != null)
                root.styleSheets.Add(styleSheet);

            var form = new ScrollView();
            form.AddToClassList("rename-objects__form");
            root.Add(form);

            var addSection = CreateSection(form, "Add");
            _prefixField = CreateTextField(addSection, "Prefix");
            _suffixField = CreateTextField(addSection, "Suffix");

            var replaceSection = CreateSection(form, "Replace");
            _replaceField = CreateTextField(replaceSection, "Replace");
            _replaceByField = CreateTextField(replaceSection, "With");

            var insertSection = CreateSection(form, "Insert");
            _insertBeforeAnchorField = CreateTextField(insertSection, "Before text");
            _insertBeforeTextField = CreateTextField(insertSection, "Insert");
            _insertAfterAnchorField = CreateTextField(insertSection, "After text");
            _insertAfterTextField = CreateTextField(insertSection, "Insert");

            var optionsSection = CreateSection(form, "Options");
            _lowercaseToggle = CreateToggle(optionsSection, "To lowercase");
            _trimLastWordToggle = CreateToggle(optionsSection, "Trim last word");

            var previewSection = CreateSection(form, "Preview");
            _previewList = new ScrollView();
            _previewList.AddToClassList("rename-objects__preview-list");
            previewSection.Add(_previewList);

            _applyButton = new Button(() => ApplyRequested?.Invoke());
            _applyButton.AddToClassList("rename-objects__apply-button");
            root.Add(_applyButton);
        }

        public void ShowPreview(IReadOnlyList<RenamePreviewItem> items)
        {
            _previewList.Clear();

            if (items.Count == 0)
            {
                _previewList.Add(CreateEmptyState());
            }
            else
            {
                foreach (var item in items)
                    _previewList.Add(CreatePreviewRow(item));
            }

            var changedCount = items.Count(item => item.IsChanged);
            _applyButton.text = changedCount > 0 ? $"Rename {changedCount} object(s)" : "Nothing to rename";
            _applyButton.SetEnabled(changedCount > 0);
        }

        private VisualElement CreateSection(VisualElement parent, string title)
        {
            var section = new VisualElement();
            section.AddToClassList("rename-objects__section");

            var titleLabel = new Label(title);
            titleLabel.AddToClassList("rename-objects__section-title");
            section.Add(titleLabel);

            parent.Add(section);
            return section;
        }

        private TextField CreateTextField(VisualElement parent, string label)
        {
            var field = new TextField(label);
            field.AddToClassList("rename-objects__field");
            field.RegisterValueChangedCallback(_ => NotifyRulesChanged());
            parent.Add(field);
            return field;
        }

        private Toggle CreateToggle(VisualElement parent, string label)
        {
            var toggle = new Toggle(label);
            toggle.AddToClassList("rename-objects__field");
            toggle.RegisterValueChangedCallback(_ => NotifyRulesChanged());
            parent.Add(toggle);
            return toggle;
        }

        private void NotifyRulesChanged()
        {
            RulesChanged?.Invoke(new RenameRules(
                _prefixField.value,
                _suffixField.value,
                _replaceField.value,
                _replaceByField.value,
                _insertBeforeAnchorField.value,
                _insertBeforeTextField.value,
                _insertAfterAnchorField.value,
                _insertAfterTextField.value,
                _lowercaseToggle.value,
                _trimLastWordToggle.value));
        }

        private static VisualElement CreatePreviewRow(RenamePreviewItem item)
        {
            var target = item.Target;

            var row = new Button(() => EditorGUIUtility.PingObject(target)) { text = string.Empty };
            row.AddToClassList("rename-objects__preview-row");

            var icon = new Image
            {
                image = AssetPreview.GetMiniThumbnail(target),
                scaleMode = ScaleMode.ScaleToFit
            };
            icon.AddToClassList("rename-objects__preview-icon");
            row.Add(icon);

            var currentLabel = new Label(item.CurrentName);
            currentLabel.AddToClassList("rename-objects__preview-current");
            row.Add(currentLabel);

            if (item.IsChanged)
            {
                var arrow = new Label("→");
                arrow.AddToClassList("rename-objects__preview-arrow");
                row.Add(arrow);

                var newLabel = new Label(item.NewName);
                newLabel.AddToClassList("rename-objects__preview-new");
                row.Add(newLabel);
            }

            return row;
        }

        private static VisualElement CreateEmptyState()
        {
            var container = new VisualElement();
            container.AddToClassList("rename-objects__empty");

            var label = new Label("Select objects in Hierarchy or Project");
            label.AddToClassList("rename-objects__empty-label");
            container.Add(label);

            return container;
        }
    }
}
