#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VFX.Scripts
{
    [CustomEditor(typeof(BoltTestSpawner))]
    public class BoltTestSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Separator();
            if (GUILayout.Button("Respawn Bolt", GUILayout.Height(40))) {
                ((BoltTestSpawner)target).Respawn();
            }
            EditorGUILayout.Separator();
            if (GUILayout.Button("Dissipate Bolt", GUILayout.Height(40))) {
                ((BoltTestSpawner)target).Dissipate();
            }
        }
    }
}

#endif