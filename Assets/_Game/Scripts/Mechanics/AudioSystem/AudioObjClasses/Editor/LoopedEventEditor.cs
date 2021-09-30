using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioSystem.SFXLoop))]
public class LoopedEventEditor : Editor
{
    private AudioSystem.SFXLoop sfxLoop;

    SerializedProperty numCycleProperty;
    SerializedProperty enableFiniteLoopingProperty;

    [SerializeField] private AudioSource previewer;

    void OnEnable()
    {
        numCycleProperty = serializedObject.FindProperty("NumCycles");
        enableFiniteLoopingProperty = serializedObject.FindProperty("FiniteLoopingEnabled");

        previewer = EditorUtility.CreateGameObjectWithHideFlags
            ("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
    }

    void OnDisable()
    {
        DestroyImmediate(previewer.gameObject);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        sfxLoop = target as AudioSystem.SFXLoop;

        base.OnInspectorGUI();

        EditorGUILayout.Space(10);

        GUILayout.Label("Loop Settings", EditorStyles.boldLabel);

        enableFiniteLoopingProperty.boolValue =
            EditorGUILayout.ToggleLeft("Enable Finite Looping", enableFiniteLoopingProperty.boolValue);

        if (enableFiniteLoopingProperty.boolValue == true)
        {
            sfxLoop.IsLoopedInfinitely = false;

            numCycleProperty.intValue =
                EditorGUILayout.IntField("Loops for # of Cycles", numCycleProperty.intValue);
        }
        else
        {
            numCycleProperty.intValue = 0;
            sfxLoop.IsLoopedInfinitely = true;
        }

        DrawPreviewButton();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPreviewButton()
    {
        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

        GUILayout.Space(20);

        if (GUILayout.Button("Preview"))
        {
            ((AudioSystem.SFXLoop)target).Preview(previewer);
        }
        if (GUILayout.Button("Stop Preview"))
        {
            ((AudioSystem.SFXLoop)target).StopPreview(previewer);
        }
        EditorGUI.EndDisabledGroup();
    }
}
