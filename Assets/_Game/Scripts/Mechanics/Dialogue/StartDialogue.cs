using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class StartDialogue : MonoBehaviour
{
    [SerializeField] private DialogueRunner runner;
    void Start()
    {
        if(runner == null)
            runner = FindObjectOfType<DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {
        var kbm = Keyboard.current;
        if(kbm.eKey.wasPressedThisFrame)
            Debug.Log("Pressed");
    }

    public void NPCInteract(InputAction.CallbackContext value) {
        Debug.Log("Jumped");
        if(value.performed) {
            runner.StartDialogue();
        }
    }
}
