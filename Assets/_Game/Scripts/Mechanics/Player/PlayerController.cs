using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    private CharacterController controller;

    [Header("Horizontal Movement")]
    [Range(0,50)] public float moveSpeed;

    private Vector3 moveVelocity;
    private Vector3 xzInput;

    [Header("Vertical Movement")]
    [SerializeField] [Range(0,20)] private float jumpForce;
    [SerializeField] [Range(0,50)] private float risingGravity, fallingGravity;
    [SerializeField] [Range(0,1)] private float floatTime;
    private bool flag_jump, flag_canFloat;

    [Header("General Control")]
    public UnityEvent OnPlayerDeath;
    public bool flag_cantAct;

#pragma warning restore 0649

    // -------------------------------------------------------------------------------------------

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate() {
        // Movement
        if(!flag_cantAct) {
            // XZ Axis
            moveVelocity = (((xzInput.x * transform.right) + (xzInput.z * transform.forward)) * moveSpeed) + (moveVelocity.y * transform.up);

            // Y Axis
            if(flag_jump) { // Jump
                moveVelocity.y = jumpForce;
                flag_jump = false;
            } else { // Gravity
                // TODO - float
                moveVelocity.y -= (moveVelocity.y > 0 ? risingGravity : fallingGravity) * Time.fixedDeltaTime;
            }

            // Apply
            controller.Move(moveVelocity * Time.fixedDeltaTime);
        }
    }

    // -------------------------------------------------------------------------------------------

    public void Move(InputAction.CallbackContext value) {
        Vector2 inputValue = value.ReadValue<Vector2>();
        xzInput = new Vector3(inputValue.x, 0f, inputValue.y);
        //Debug.Log(value.ReadValue<Vector2>());
    }

    public void Jump(InputAction.CallbackContext value) {
        if(value.performed) {
            //Teleport(GameObject.Find("Cube").transform);

            if(/*grounded*/true)
                flag_jump = true;
        }
    }

    public void Teleport(Transform other, Vector3 offset = default) {
        // TODO: Swap teleport player and other transform
        Vector3 oldPlayerPos = transform.position;

        controller.enabled = false;
        transform.position = other.position + offset;
        other.position = oldPlayerPos;
        controller.enabled = true;

        Debug.Log("Teleport to " + other.gameObject.name + " at " + other.position + offset, other.gameObject);
    }
}