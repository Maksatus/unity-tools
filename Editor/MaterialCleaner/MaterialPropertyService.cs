using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.MaterialCleaner
{
    public interface IMaterialPropertyService
    {
        MaterialInspection Inspect(Material material);
        void RemoveProperty(Material material, MaterialPropertyType type, string propertyName);
        int RemoveUnusedProperties(Material material, MaterialPropertyType type);
        int TransferFloatsToInts(Material material);
    }

    public class MaterialPropertyService : IMaterialPropertyService
    {
        private const string ErrorShaderName = "Hidden/InternalErrorShader";
        private const string NullShaderLabel = "NULL Shader";
        private const string PropertyNameField = "first";
        private const string PropertyValueField = "second";

        private static readonly MaterialPropertyType[] AllTypes = (MaterialPropertyType[])Enum.GetValues(typeof(MaterialPropertyType));

        public MaterialInspection Inspect(Material material)
        {
            var hasShader = HasShader(material);
            var serializedObject = new SerializedObject(material);
            var groups = new List<MaterialPropertyGroup>(AllTypes.Length);

            foreach (var type in AllTypes)
                groups.Add(InspectGroup(material, serializedObject, type, hasShader));

            return new MaterialInspection(
                material,
                hasShader ? material.shader.name : NullShaderLabel,
                hasShader,
                groups);
        }

        public void RemoveProperty(Material material, MaterialPropertyType type, string propertyName)
        {
            var serializedObject = new SerializedObject(material);
            var properties = serializedObject.FindProperty(GetPropertiesPath(type));
            if (properties == null || !properties.isArray)
                return;

            for (var i = properties.arraySize - 1; i >= 0; i--)
            {
                if (GetPropertyName(properties.GetArrayElementAtIndex(i)) != propertyName)
                    continue;

                properties.DeleteArrayElementAtIndex(i);
                Debug.Log($"Removed {type} property '{propertyName}' from material '{material.name}'");
            }

            serializedObject.ApplyModifiedProperties();
        }

        public int RemoveUnusedProperties(Material material, MaterialPropertyType type)
        {
            if (!HasShader(material))
            {
                Debug.LogError($"Material '{material.name}' doesn't have a shader");
                return 0;
            }

            var serializedObject = new SerializedObject(material);
            var properties = serializedObject.FindProperty(GetPropertiesPath(type));
            if (properties == null || !properties.isArray)
                return 0;

            var removed = 0;
            for (var i = properties.arraySize - 1; i >= 0; i--)
            {
                var propertyName = GetPropertyName(properties.GetArrayElementAtIndex(i));
                if (ShaderHasProperty(material, propertyName, type))
                    continue;

                properties.DeleteArrayElementAtIndex(i);
                removed++;
                Debug.Log($"Removed {type} property '{propertyName}' from material '{material.name}'");
            }

            if (removed > 0)
                serializedObject.ApplyModifiedProperties();

            return removed;
        }

        public int TransferFloatsToInts(Material material)
        {
            if (!HasShader(material))
            {
                Debug.LogError($"Material '{material.name}' doesn't have a shader");
                return 0;
            }

            var serializedObject = new SerializedObject(material);
            var floats = serializedObject.FindProperty(GetPropertiesPath(MaterialPropertyType.Float));
            var ints = serializedObject.FindProperty(GetPropertiesPath(MaterialPropertyType.Int));
            if (floats == null || !floats.isArray || ints == null || !ints.isArray)
                return 0;

            var transferred = 0;
            for (var intIndex = 0; intIndex < ints.arraySize; intIndex++)
            {
                var intProperty = ints.GetArrayElementAtIndex(intIndex);
                var propertyName = GetPropertyName(intProperty);

                if (ShaderHasProperty(material, propertyName, MaterialPropertyType.Float))
                {
                    Debug.LogError(
                        $"Material '{material.name}' has an Int property '{propertyName}' whereas the shader " +
                        $"'{material.shader.name}' has it as a Float property.\n" +
                        "The Int material property should be cleaned away");
                    continue;
                }

                for (var floatIndex = floats.arraySize - 1; floatIndex >= 0; floatIndex--)
                {
                    var floatProperty = floats.GetArrayElementAtIndex(floatIndex);
                    if (GetPropertyName(floatProperty) != propertyName)
                        continue;

                    var intValue = intProperty.FindPropertyRelative(PropertyValueField);
                    var floatValue = floatProperty.FindPropertyRelative(PropertyValueField);
                    intValue.intValue = Mathf.RoundToInt(floatValue.floatValue);
                    floats.DeleteArrayElementAtIndex(floatIndex);
                    transferred++;

                    Debug.Log($"Transferred Float '{propertyName}' to Int of same name in material '{material.name}'");
                }
            }

            if (transferred > 0)
                serializedObject.ApplyModifiedProperties();

            return transferred;
        }

        private static MaterialPropertyGroup InspectGroup(Material material, SerializedObject serializedObject,
            MaterialPropertyType type, bool hasShader)
        {
            var result = new List<MaterialPropertyInfo>();
            var properties = serializedObject.FindProperty(GetPropertiesPath(type));

            if (properties != null && properties.isArray)
            {
                for (var i = 0; i < properties.arraySize; i++)
                {
                    var propertyName = GetPropertyName(properties.GetArrayElementAtIndex(i));
                    var status = !hasShader
                        ? MaterialPropertyStatus.Unknown
                        : ShaderHasProperty(material, propertyName, type)
                            ? MaterialPropertyStatus.Exists
                            : MaterialPropertyStatus.OldReference;

                    result.Add(new MaterialPropertyInfo(propertyName, status));
                }
            }

            return new MaterialPropertyGroup(type, result);
        }

        private static string GetPropertiesPath(MaterialPropertyType type)
        {
            switch (type)
            {
                case MaterialPropertyType.Texture: return "m_SavedProperties.m_TexEnvs";
                case MaterialPropertyType.Int: return "m_SavedProperties.m_Ints";
                case MaterialPropertyType.Float: return "m_SavedProperties.m_Floats";
                case MaterialPropertyType.Color: return "m_SavedProperties.m_Colors";
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static bool ShaderHasProperty(Material material, string propertyName, MaterialPropertyType type)
        {
            switch (type)
            {
                case MaterialPropertyType.Texture: return material.HasTexture(propertyName);
                case MaterialPropertyType.Int: return material.HasInteger(propertyName);
                case MaterialPropertyType.Float: return material.HasFloat(propertyName);
                case MaterialPropertyType.Color: return material.HasColor(propertyName);
                default: return false;
            }
        }

        private static string GetPropertyName(SerializedProperty property)
        {
            return property.FindPropertyRelative(PropertyNameField).stringValue;
        }

        private static bool HasShader(Material material)
        {
            return material.shader != null && material.shader.name != ErrorShaderName;
        }
    }
}
