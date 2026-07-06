using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Editor.MaterialCleaner.Data
{
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
