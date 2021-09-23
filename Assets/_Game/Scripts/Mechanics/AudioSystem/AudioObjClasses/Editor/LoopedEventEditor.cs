using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(AudioSystem.SFXLoop))]
public class LoopedEventEditor : Editor
{
    private AudioSystem.SFXLoop sfxLoop;

    SerializedProperty numCycleProperty;
    SerializedProperty enableFiniteLoopingProperty;

    void OnEnable()
    {
        numCycleProperty = serializedObject.FindProperty("NumCycles");
        enableFiniteLoopingProperty = serializedObject.FindProperty("FiniteLoopingEnabled");
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

        serializedObject.ApplyModifiedProperties();
    }
}
