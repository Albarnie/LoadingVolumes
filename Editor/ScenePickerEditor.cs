using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(ScenePicker))]
public class ScenePickerEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;

        string oldScenePath = property.FindPropertyRelative("scenePath").stringValue;
        var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(oldScenePath);

        //Field for the scene override
        var sceneField = new ObjectField();
        sceneField.objectType = typeof(SceneAsset);
        sceneField.value = oldScene;
        sceneField.RegisterCallback<ChangeEvent<Object>, SerializedProperty>(GetScenePath, property, TrickleDown.TrickleDown);

        //Field for whether the scene should be enabled
        var enabledField = new Toggle();
        enabledField.value = property.FindPropertyRelative("loaded").boolValue;
        enabledField.RegisterCallback<ChangeEvent<bool>, SerializedProperty>(SetSceneEnabled, property, TrickleDown.TrickleDown);

        // Add fields to the container.
        container.Add(sceneField);
        container.Add(enabledField);

        return container;
    }

    void GetScenePath(ChangeEvent<Object> evt, SerializedProperty property)
    {
        if (evt.newValue != null)
        {
            property.FindPropertyRelative("scenePath").stringValue = AssetDatabase.GetAssetPath(evt.newValue);
            property.FindPropertyRelative("sceneName").stringValue = evt.newValue.name;
        }
        property.serializedObject.ApplyModifiedProperties();
    }

    void SetSceneEnabled (ChangeEvent<bool> evt, SerializedProperty property)
    {
        property.FindPropertyRelative("loaded").boolValue = evt.newValue;
        property.serializedObject.ApplyModifiedProperties();
    }
}
