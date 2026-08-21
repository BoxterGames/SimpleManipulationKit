using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleManipulationKit.Editor
{
    [CustomPropertyDrawer(typeof(Attributes))]
    public sealed class SerializeReferenceDropdownDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Type, Type[]> Cache = new();

        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.EndProperty();
                return;
            }

            var types = GetTypes(fieldInfo.FieldType);

            if (types.Length == 0)
            {
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.EndProperty();
                return;
            }

            var currentType = GetSharedType(property, out var mixed);

            var names = new[] { "None" }
                .Concat(types.Select(x => ObjectNames.NicifyVariableName(x.Name)))
                .ToArray();

            var index = currentType == null
                ? 0
                : Array.IndexOf(types, currentType) + 1;

            EditorGUI.showMixedValue = mixed;

            EditorGUI.BeginChangeCheck();

            var popupRect = EditorGUI.PrefixLabel(position, label);
            var newIndex = EditorGUI.Popup(popupRect, index, names);

            var changed = EditorGUI.EndChangeCheck();

            EditorGUI.showMixedValue = false;

            if (changed)
            {
                var type = newIndex == 0
                    ? null
                    : types[newIndex - 1];

                ApplyType(property, type);
            }

            EditorGUI.EndProperty();
        }

        private static Type GetSharedType(
            SerializedProperty property,
            out bool mixed)
        {
            mixed = false;

            Type sharedType = null;
            var first = true;

            foreach (var target in property.serializedObject.targetObjects)
            {
                using var serializedObject = new SerializedObject(target);
                var targetProperty =
                    serializedObject.FindProperty(property.propertyPath);

                var type = targetProperty?.managedReferenceValue?.GetType();

                if (first)
                {
                    sharedType = type;
                    first = false;
                    continue;
                }

                if (type != sharedType)
                {
                    mixed = true;
                    return null;
                }
            }

            return sharedType;
        }

        private static void ApplyType(
            SerializedProperty property,
            Type type)
        {
            var propertyPath = property.propertyPath;

            foreach (var target in property.serializedObject.targetObjects)
            {
                using var serializedObject = new SerializedObject(target);

                var targetProperty =
                    serializedObject.FindProperty(propertyPath);

                if (targetProperty == null)
                    continue;

                targetProperty.managedReferenceValue = type == null
                    ? null
                    : Activator.CreateInstance(type);

                serializedObject.ApplyModifiedProperties();
            }

            property.serializedObject.Update();
        }

        private static Type[] GetTypes(Type baseType)
        {
            if (Cache.TryGetValue(baseType, out var result))
                return result;

            result = TypeCache
                .GetTypesDerivedFrom(baseType)
                .Where(x =>
                    !x.IsAbstract &&
                    !x.IsInterface &&
                    !x.IsGenericType)
                .OrderBy(x => x.Name)
                .ToArray();

            Cache[baseType] = result;
            return result;
        }
    }
}