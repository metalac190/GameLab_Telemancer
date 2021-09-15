using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    private CharacterController controller;



    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate() {

    }

    // ------------------

    public void Move(InputAction.CallbackContext value) {
        Debug.Log(value.ReadValue<Vector2>());
    }

}