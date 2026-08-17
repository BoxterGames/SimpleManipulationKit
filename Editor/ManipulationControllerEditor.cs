using System;
using SimpleManipulationKit.Internal;
using UnityEditor;

namespace SimpleManipulationKit.Editor
{
    public abstract class ManipulationControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string pendingPath = null;
            Type pendingType = null;

            var property = serializedObject.GetIterator();
            var enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (SerializeReferenceDropdownUtility.IsSerializeReference(property))
                {
                    if (SerializeReferenceDropdownUtility.DrawProperty(property, out var selectedType))
                    {
                        pendingPath = property.propertyPath;
                        pendingType = selectedType;
                    }

                    continue;
                }

                using (new EditorGUI.DisabledScope(property.name == "m_Script"))
                {
                    EditorGUILayout.PropertyField(property, true);
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (pendingType != null)
            {
                SerializeReferenceDropdownUtility.ApplyType(serializedObject, pendingPath, pendingType);
            }
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(DragController))]
    public sealed class DragControllerEditor : ManipulationControllerEditor { }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(DragController3D))]
    public sealed class DragController3DEditor : ManipulationControllerEditor { }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(SelectionController))]
    public sealed class SelectionControllerEditor : ManipulationControllerEditor { }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(SelectionController3D))]
    public sealed class SelectionController3DEditor : ManipulationControllerEditor { }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(MarqueSelectController))]
    public sealed class MarqueSelectControllerEditor : ManipulationControllerEditor { }
}
