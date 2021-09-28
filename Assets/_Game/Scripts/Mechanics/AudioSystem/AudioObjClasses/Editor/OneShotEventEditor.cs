using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AudioSystem
{
    [CustomEditor(typeof(SFXOneShot), true)]
    public class OneShotEventEditor : Editor
    {

        [SerializeField] private AudioSource previewer;

        public void OnEnable()
        {
            previewer = EditorUtility.CreateGameObjectWithHideFlags
                ("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
        }

        private void OnDisable()
        {
            DestroyImmediate(previewer.gameObject);
        }

        public override void OnInspectorGUI()
        {
            var soundEvent = target as SFXOneShot;

            DrawDefaultInspector();

            DrawPreviewButton();
        }

        private void DrawPreviewButton()
        {
            EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

            GUILayout.Space(20);

            if (GUILayout.Button("Preview"))
            {
                ((SFXOneShot)target).Preview(previewer);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}