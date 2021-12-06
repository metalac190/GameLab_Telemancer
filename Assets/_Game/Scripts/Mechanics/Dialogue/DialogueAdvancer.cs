using System;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;

public class DialogueAdvancer : MonoBehaviour
{
    private CustomDialogueUI dialogueUI;
    private DialogueRunner dialogueRunner;
    private bool firstDialogue = true;
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
                dialogueUI.DialogueExited();
                dialogueRunner.IsDialogueRunning = false;
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (!firstDialogue)
                {
                    dialogueUI.MarkLineComplete();
                }
                else
                {
                    firstDialogue = false;
                    dialogueUI.onDialogueEnd.AddListener(ResetDialogue);
                }
            }
        }
    }

    void ResetDialogue()
    {
        firstDialogue = true;
        dialogueUI.onDialogueEnd.RemoveListener(ResetDialogue);
    }
}
