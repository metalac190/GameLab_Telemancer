using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mechanics.Player;
using Yarn.Unity;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueRunner runner;
    [SerializeField] private PlayerState player;
    [SerializeField] private TextMeshProUGUI dialogueText = null, speaker = null;
    void Start()
    {
        // Null checks
        if (runner == null)
            runner = FindObjectOfType<DialogueRunner>();
        if (player == null)
            player = FindObjectOfType<PlayerState>();
    }

    public void DialogueStart()
    {
        // Remove all player control when we're in dialogue
        player.GamePaused(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void DialogueEnd()
    {
        // Allow player control once dialogue is finished
        player.GamePaused(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetText(string textAndSpeaker)
    {
        string[] fullLine = textAndSpeaker.Split(':');
        string text = fullLine[1].Trim(' ');
        string name = fullLine[0];

        dialogueText.text = text;
        speaker.text = name;
    }
}