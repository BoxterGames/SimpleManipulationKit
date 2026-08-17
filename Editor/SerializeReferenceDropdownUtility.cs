using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SimpleManipulationKit.Editor
{
    public static class SerializeReferenceDropdownUtility
    {
        public static bool IsSerializeReference(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                return false;
            }

            return GetField(property)?.IsDefined(typeof(SerializeReference), false) == true;
        }

        public static bool DrawProperty(SerializedProperty property, out Type selectedType)
        {
            selectedType = null;

            var fieldType = GetField(property)?.FieldType;
            var types = fieldType == null ? new List<Type>() : GetTypes(fieldType);

            var rect = EditorGUILayout.GetControlRect();
            var label = new GUIContent(property.displayName);

            if (types.Count == 0)
            {
                EditorGUI.PropertyField(rect, property, label, true);
                return false;
            }

            EditorGUI.BeginProperty(rect, label, property);

            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
            var popupRect = new Rect(
                rect.x + EditorGUIUtility.labelWidth,
                rect.y,
                rect.width - EditorGUIUtility.labelWidth,
                rect.height);

            EditorGUI.PrefixLabel(labelRect, label);
            var changed = DrawTypePopup(popupRect, property, types, out selectedType);

            EditorGUI.EndProperty();

            return changed;
        }

        public static void ApplyType(SerializedObject serializedObject, string propertyPath, Type type)
        {
            foreach (var target in serializedObject.targetObjects)
            {
                using var targetSerializedObject = new SerializedObject(target);
                var property = targetSerializedObject.FindProperty(propertyPath);
                if (property == null)
                {
                    continue;
                }

                property.managedReferenceValue = Activator.CreateInstance(type);
                targetSerializedObject.ApplyModifiedProperties();
            }

            serializedObject.Update();
        }

        private static bool DrawTypePopup(Rect position, SerializedProperty property, List<Type> types, out Type selectedType)
        {
            selectedType = null;

            var currentType = GetSharedType(property, out var hasMultipleValues);
            var selectedIndex = currentType == null ? -1 : types.IndexOf(currentType);
            var names = types.Select(type => ObjectNames.NicifyVariableName(type.Name)).ToArray();

            EditorGUI.showMixedValue = hasMultipleValues;
            EditorGUI.BeginChangeCheck();
            var newIndex = EditorGUI.Popup(position, selectedIndex, names);
            var changed = EditorGUI.EndChangeCheck();
            EditorGUI.showMixedValue = false;

            if (!changed || newIndex < 0 || (!hasMultipleValues && newIndex == selectedIndex))
            {
                return false;
            }

            selectedType = types[newIndex];
            return true;
        }

        private static Type GetSharedType(SerializedProperty property, out bool hasMultipleValues)
        {
            hasMultipleValues = false;

            Type shared = null;
            var first = true;

            foreach (var target in property.serializedObject.targetObjects)
            {
                using var targetSerializedObject = new SerializedObject(target);
                var targetProperty = targetSerializedObject.FindProperty(property.propertyPath);
                var type = targetProperty?.managedReferenceValue?.GetType();

                if (first)
                {
                    shared = type;
                    first = false;
                    continue;
                }

                if (type != shared)
                {
                    hasMultipleValues = true;
                    return null;
                }
            }

            return shared;
        }

        private static List<Type> GetTypes(Type baseType)
        {
            return TypeCache.GetTypesDerivedFrom(baseType)
                .Where(type => !type.IsAbstract && !type.IsInterface)
                .OrderBy(type => type.Name)
                .ToList();
        }

        private static FieldInfo GetField(SerializedProperty property)
        {
            var targetType = property.serializedObject.targetObject.GetType();
            return targetType.GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
    }
}
