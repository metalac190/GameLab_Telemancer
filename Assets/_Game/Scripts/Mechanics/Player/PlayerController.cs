using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    private CharacterController controller;

    public float moveSpeed;

    private Vector3 moveVelocity;
    private Vector2 xzInput;

    // -------------------------------------------------------------------------------------------

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate() {
        moveVelocity = ((xzInput.x * transform.right) + (xzInput.y * transform.forward)) * moveSpeed;
        controller.Move(moveVelocity * Time.deltaTime);
    }

    // -------------------------------------------------------------------------------------------

    public void Move(InputAction.CallbackContext value) {
        xzInput = value.ReadValue<Vector2>();
        //Debug.Log(value.ReadValue<Vector2>());
    }

}