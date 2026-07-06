using System.Collections.Generic;

namespace UnityTools.Editor.MaterialCleaner.Data
{
    public readonly struct MaterialPropertyGroup
    {
        public MaterialSavedPropertyType Type { get; }
        public IReadOnlyList<MaterialPropertyInfo> Properties { get; }

        public MaterialPropertyGroup(MaterialSavedPropertyType type, IReadOnlyList<MaterialPropertyInfo> properties)
        {
            Type = type;
            Properties = properties;
        }
    }
}
