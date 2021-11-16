using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;

public class DialogueAdvancer : MonoBehaviour
{
    private CustomDialogueUI dialogueUI;
    private DialogueRunner dialogueRunner;
    void Start()
    {
        dialogueUI = GetComponent<CustomDialogueUI>();
        dialogueRunner = GetComponent<DialogueRunner>();
    }

    void Update()
    {
        if (dialogueRunner.IsDialogueRunning)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                dialogueUI.DialogueComplete();
                dialogueRunner.IsDialogueRunning = false;
            }
            if (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                dialogueUI.MarkLineComplete();
            }
        }
    }
}
