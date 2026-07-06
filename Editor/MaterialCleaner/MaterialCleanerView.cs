using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor.MaterialCleaner
{
    public interface IMaterialCleanerView
    {
        event Action<Material, MaterialPropertyType, string> RemovePropertyRequested;
        event Action<MaterialPropertyType> RemoveUnusedOfTypeRequested;
        event Action RemoveAllUnusedRequested;
        event Action TransferFloatsRequested;

        void ShowMaterials(IReadOnlyList<MaterialInspection> materials);
    }

    public class MaterialCleanerView : IMaterialCleanerView
    {
        private const string StyleSheetPath = "Packages/com.maksatus.unity-tools/Editor/MaterialCleaner/MaterialCleanerView.uss";

        private const string MaterialIconName = "Material Icon";
        private const string ShaderIconName = "Shader Icon";
        private const string EmptyIconName = "d_SearchWindowIcon";

        private static readonly Dictionary<MaterialPropertyType, string> GroupTitles = new()
        {
            { MaterialPropertyType.Texture, "Textures" },
            { MaterialPropertyType.Int, "Ints" },
            { MaterialPropertyType.Float, "Floats" },
            { MaterialPropertyType.Color, "Colors" }
        };

        private readonly ScrollView _materialList;
        private readonly Label _summaryLabel;

        public event Action<Material, MaterialPropertyType, string> RemovePropertyRequested;
        public event Action<MaterialPropertyType> RemoveUnusedOfTypeRequested;
        public event Action RemoveAllUnusedRequested;
        public event Action TransferFloatsRequested;

        public MaterialCleanerView(VisualElement root)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(StyleSheetPath);
            if (styleSheet != null)
                root.styleSheets.Add(styleSheet);

            var toolbar = CreateToolbar(out _summaryLabel);
            root.Add(toolbar);

            _materialList = new ScrollView();
            _materialList.AddToClassList("material-cleaner__list");
            root.Add(_materialList);
        }

        public void ShowMaterials(IReadOnlyList<MaterialInspection> materials)
        {
            _materialList.Clear();
            UpdateSummary(materials);

            if (materials.Count == 0)
            {
                _materialList.Add(CreateEmptyState());
                return;
            }

            foreach (var inspection in materials)
                _materialList.Add(CreateMaterialCard(inspection));
        }

        private VisualElement CreateToolbar(out Label summaryLabel)
        {
            var toolbar = new VisualElement();
            toolbar.AddToClassList("material-cleaner__toolbar");

            var actionsRow = new VisualElement();
            actionsRow.AddToClassList("material-cleaner__toolbar-row");
            toolbar.Add(actionsRow);

            var transferButton = new Button(() => TransferFloatsRequested?.Invoke())
            {
                text = "Transfer Floats → Ints",
                tooltip = "Move saved Float values into Int properties of the same name"
            };
            transferButton.AddToClassList("material-cleaner__action-button");
            actionsRow.Add(transferButton);

            var removeAllButton = new Button(() => RemoveAllUnusedRequested?.Invoke())
            {
                text = "Remove All Old",
                tooltip = "Remove every property the shader no longer uses"
            };
            removeAllButton.AddToClassList("material-cleaner__action-button");
            removeAllButton.AddToClassList("material-cleaner__action-button--danger");
            actionsRow.Add(removeAllButton);

            var typesRow = new VisualElement();
            typesRow.AddToClassList("material-cleaner__toolbar-row");
            toolbar.Add(typesRow);

            var removeLabel = new Label("Remove old:");
            removeLabel.AddToClassList("material-cleaner__toolbar-label");
            typesRow.Add(removeLabel);

            foreach (var (type, title) in GroupTitles)
            {
                var typeButton = new Button(() => RemoveUnusedOfTypeRequested?.Invoke(type)) { text = title };
                typeButton.AddToClassList("material-cleaner__type-button");
                typesRow.Add(typeButton);
            }

            summaryLabel = new Label();
            summaryLabel.AddToClassList("material-cleaner__summary");
            toolbar.Add(summaryLabel);

            return toolbar;
        }

        private void UpdateSummary(IReadOnlyList<MaterialInspection> materials)
        {
            var oldCount = materials
                .SelectMany(inspection => inspection.Groups)
                .SelectMany(group => group.Properties)
                .Count(property => property.Status == MaterialPropertyStatus.OldReference);

            _summaryLabel.text = $"Materials: {materials.Count}    Old references: {oldCount}";
            _summaryLabel.EnableInClassList("material-cleaner__summary--has-old", oldCount > 0);
        }

        private VisualElement CreateMaterialCard(MaterialInspection inspection)
        {
            var card = new VisualElement();
            card.AddToClassList("material-cleaner__card");

            card.Add(CreateCardHeader(inspection));

            foreach (var group in inspection.Groups)
            {
                if (group.Properties.Count == 0)
                    continue;

                card.Add(CreateGroupSection(inspection.Material, group));
            }

            return card;
        }

        private static VisualElement CreateCardHeader(MaterialInspection inspection)
        {
            var header = new VisualElement();
            header.AddToClassList("material-cleaner__card-header");

            var material = inspection.Material;

            var materialButton = CreatePingButton(material, material.name, MaterialIconName,
                AssetDatabase.GetAssetPath(material));
            header.Add(materialButton);

            if (inspection.HasShader)
            {
                var shaderButton = CreatePingButton(material.shader, inspection.ShaderName, ShaderIconName,
                    "Ping shader");
                shaderButton.AddToClassList("material-cleaner__ping-button--shader");
                header.Add(shaderButton);
            }
            else
            {
                var nullShaderLabel = new Label(inspection.ShaderName);
                nullShaderLabel.AddToClassList("material-cleaner__null-shader");
                header.Add(nullShaderLabel);
            }

            return header;
        }

        private static Button CreatePingButton(Object target, string text, string iconName, string tooltip)
        {
            var button = new Button(() => EditorGUIUtility.PingObject(target))
            {
                text = string.Empty,
                tooltip = tooltip
            };
            button.AddToClassList("material-cleaner__ping-button");

            button.Add(CreateIcon(iconName, "material-cleaner__header-icon"));

            var label = new Label(text);
            label.AddToClassList("material-cleaner__ping-label");
            button.Add(label);

            return button;
        }

        private VisualElement CreateGroupSection(Material material, MaterialPropertyGroup group)
        {
            var section = new VisualElement();
            section.AddToClassList("material-cleaner__group");

            var title = new Label($"{GroupTitles[group.Type]} ({group.Properties.Count})");
            title.AddToClassList("material-cleaner__group-title");
            section.Add(title);

            foreach (var property in group.Properties)
                section.Add(CreatePropertyRow(material, group.Type, property));

            return section;
        }

        private VisualElement CreatePropertyRow(Material material, MaterialPropertyType type,
            MaterialPropertyInfo property)
        {
            var row = new VisualElement();
            row.AddToClassList("material-cleaner__property-row");

            var nameLabel = new Label(property.Name);
            nameLabel.AddToClassList("material-cleaner__property-name");
            row.Add(nameLabel);

            switch (property.Status)
            {
                case MaterialPropertyStatus.Exists:
                    row.Add(CreateStatusBadge("Exists", "material-cleaner__status--exists"));
                    break;

                case MaterialPropertyStatus.Unknown:
                    row.Add(CreateStatusBadge("Unknown", "material-cleaner__status--unknown"));
                    break;

                case MaterialPropertyStatus.OldReference:
                    row.Add(CreateStatusBadge("Old Reference", "material-cleaner__status--old"));

                    var removeButton = new Button(() => RemovePropertyRequested?.Invoke(material, type, property.Name))
                    {
                        text = "Remove"
                    };
                    removeButton.AddToClassList("material-cleaner__remove-button");
                    row.Add(removeButton);
                    break;
            }

            return row;
        }

        private static Label CreateStatusBadge(string text, string modifierClass)
        {
            var badge = new Label(text);
            badge.AddToClassList("material-cleaner__status");
            badge.AddToClassList(modifierClass);
            return badge;
        }

        private static VisualElement CreateEmptyState()
        {
            var container = new VisualElement();
            container.AddToClassList("material-cleaner__empty");

            container.Add(CreateIcon(EmptyIconName, "material-cleaner__empty-icon"));

            var label = new Label("Select materials in the Project window");
            label.AddToClassList("material-cleaner__empty-label");
            container.Add(label);

            return container;
        }

        private static Image CreateIcon(string iconName, string className)
        {
            var icon = new Image
            {
                image = EditorGUIUtility.IconContent(iconName).image,
                scaleMode = ScaleMode.ScaleToFit
            };
            icon.AddToClassList(className);
            return icon;
        }
    }
}
