using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CanEditMultipleObjects]
[CustomEditor(typeof(LoadingVolume), true)]
public class LoadingVolumeEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new VisualElement();

        container.Add(new PropertyField(serializedObject.FindProperty("isGlobal")));
        container.Add(new PropertyField(serializedObject.FindProperty("priority")));
        container.Add(new PropertyField(serializedObject.FindProperty("scenes")));

        return container;
    }
}
