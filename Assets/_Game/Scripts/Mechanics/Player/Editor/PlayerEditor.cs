using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        PlayerController pc = target as PlayerController;

        if(GUILayout.Button("Teleport With Transform")) {
            pc.DebugTeleportWithTransform();
        }
        if(GUILayout.Button("Teleport To Transform")) {
            pc.DebugTeleportToTransform();
        }
        if(GUILayout.Button("Teleport To Vector3")) {
            pc.DebugTeleportToVector3();
        }
    }

}