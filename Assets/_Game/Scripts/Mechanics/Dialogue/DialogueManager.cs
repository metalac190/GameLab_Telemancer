﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mechanics.Player;
using Yarn.Unity;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueRunner runner;
    [SerializeField] private PlayerController player;
    [SerializeField] private float interactionRadius = 5;
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
                runner.StartDialogue(target.talkToNode);
            }
        }
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
}
