using System.Collections.Generic;
using UnityEngine;

namespace Editor.MaterialCleaner
{
    public enum MaterialPropertyType
    {
        Texture,
        Int,
        Float,
        Color
    }

    public enum MaterialPropertyStatus
    {
        Exists,
        OldReference,
        Unknown
    }

    public readonly struct MaterialPropertyInfo
    {
        public string Name { get; }
        public MaterialPropertyStatus Status { get; }

        public MaterialPropertyInfo(string name, MaterialPropertyStatus status)
        {
            Name = name;
            Status = status;
        }
    }

    public readonly struct MaterialPropertyGroup
    {
        public MaterialPropertyType Type { get; }
        public IReadOnlyList<MaterialPropertyInfo> Properties { get; }

        public MaterialPropertyGroup(MaterialPropertyType type, IReadOnlyList<MaterialPropertyInfo> properties)
        {
            Type = type;
            Properties = properties;
        }
    }

    public readonly struct MaterialInspection
    {
        public Material Material { get; }
        public string ShaderName { get; }
        public bool HasShader { get; }
        public IReadOnlyList<MaterialPropertyGroup> Groups { get; }

        public MaterialInspection(Material material, string shaderName, bool hasShader,
            IReadOnlyList<MaterialPropertyGroup> groups)
        {
            Material = material;
            ShaderName = shaderName;
            HasShader = hasShader;
            Groups = groups;
        }
    }
}
