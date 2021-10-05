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
    [SerializeField] private PlayerController player;
    [SerializeField] private float interactionRadius = 5;
    [SerializeField] private TextMeshProUGUI dialogueText, speaker;
    void Start()
    {
        if (runner == null)
            runner = FindObjectOfType<DialogueRunner>();
    }

    public void CheckForNearbyNPC(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            Debug.Log("performed");
            var allParticipants = new List<NPC>(FindObjectsOfType<NPC>());
            var target = allParticipants.Find(delegate (NPC p)
            {
                return string.IsNullOrEmpty(p.talkToNode) == false &&
                (p.transform.position - player.gameObject.transform.position)
                .magnitude <= interactionRadius;
            });
            if (target != null)
            {
                // Kick off the dialogue at this node.
                runner.StartDialogue(RandomTedTalk());
            }
        }
    }

    string RandomTedTalk()
    {
        string nodeString = "TedTalk";
        nodeString += Random.Range(1, 18);
        return nodeString;
    }

    public void DialogueStart()
    {
        // Remove all player control when we're in dialogue
        player.flag_cantAct = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void DialogueEnd()
    {
        // Allow player control once dialogue is finished
        player.flag_cantAct = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetText(string textAndSpeaker)
    {
        Debug.Log(textAndSpeaker);
        string[] fullLine = textAndSpeaker.Split(':');
        string text = fullLine[1].Trim(' ');
        string name = fullLine[0];

        dialogueText.text = text;
        speaker.text = name;
    }
}
