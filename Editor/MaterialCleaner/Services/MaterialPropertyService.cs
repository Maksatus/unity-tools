using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityTools.Editor.MaterialCleaner.Data;
using static UnityTools.Editor.MaterialCleaner.MaterialCleanerConstants;

namespace UnityTools.Editor.MaterialCleaner.Services
{
    public class MaterialPropertyService : IMaterialPropertyService
    {
        private static readonly MaterialSavedPropertyType[] AllTypes = (MaterialSavedPropertyType[])Enum.GetValues(typeof(MaterialSavedPropertyType));

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

        public void RemoveProperty(Material material, MaterialSavedPropertyType type, string propertyName)
        {
            var serializedObject = new SerializedObject(material);
            var properties = serializedObject.FindProperty(GetPropertiesPath(type));
            if (properties is not { isArray: true }) return;

            for (var i = properties.arraySize - 1; i >= 0; i--)
            {
                if (GetPropertyName(properties.GetArrayElementAtIndex(i)) != propertyName)
                    continue;

                properties.DeleteArrayElementAtIndex(i);
                Debug.Log($"Removed {type} property '{propertyName}' from material '{material.name}'");
            }

            serializedObject.ApplyModifiedProperties();
        }

        public int RemoveUnusedProperties(Material material, MaterialSavedPropertyType type)
        {
            if (!HasShader(material))
            {
                Debug.LogError($"Material '{material.name}' doesn't have a shader");
                return 0;
            }

            var serializedObject = new SerializedObject(material);
            var properties = serializedObject.FindProperty(GetPropertiesPath(type));
            if (properties is not { isArray: true })
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
            var floats = serializedObject.FindProperty(GetPropertiesPath(MaterialSavedPropertyType.Float));
            var ints = serializedObject.FindProperty(GetPropertiesPath(MaterialSavedPropertyType.Int));
            
            if (floats is not { isArray: true } || ints is not { isArray: true }) return 0;

            var transferred = 0;
            for (var intIndex = 0; intIndex < ints.arraySize; intIndex++)
            {
                var intProperty = ints.GetArrayElementAtIndex(intIndex);
                var propertyName = GetPropertyName(intProperty);

                if (ShaderHasProperty(material, propertyName, MaterialSavedPropertyType.Float))
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

        private static MaterialPropertyGroup InspectGroup(Material material, SerializedObject serializedObject, MaterialSavedPropertyType type, bool hasShader)
        {
            var result = new List<MaterialPropertyInfo>();
            var properties = serializedObject.FindProperty(GetPropertiesPath(type));

            if (properties is not { isArray: true }) return new MaterialPropertyGroup(type, result);
            
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

            return new MaterialPropertyGroup(type, result);
        }

        private static string GetPropertiesPath(MaterialSavedPropertyType type)
        {
            return type switch
            {
                MaterialSavedPropertyType.Texture => "m_SavedProperties.m_TexEnvs",
                MaterialSavedPropertyType.Int => "m_SavedProperties.m_Ints",
                MaterialSavedPropertyType.Float => "m_SavedProperties.m_Floats",
                MaterialSavedPropertyType.Color => "m_SavedProperties.m_Colors",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private static bool ShaderHasProperty(Material material, string propertyName, MaterialSavedPropertyType type)
        {
            return type switch
            {
                MaterialSavedPropertyType.Texture => material.HasTexture(propertyName),
                MaterialSavedPropertyType.Int => material.HasInteger(propertyName),
                MaterialSavedPropertyType.Float => material.HasFloat(propertyName),
                MaterialSavedPropertyType.Color => material.HasColor(propertyName),
                _ => false
            };
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
