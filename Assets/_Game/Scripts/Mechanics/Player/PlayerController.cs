using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    private CharacterController controller;

    [Header("Horizontal Movement")]
    public float moveSpeed;

    private Vector3 moveVelocity;
    private Vector3 xzInput;

    [Header("Vertical Movement")]
    [SerializeField] private int jumpForce;

    [Header("General Control")]
    public UnityEvent OnPlayerDeath;
    public bool flag_cantAct;

#pragma warning restore 0649

    // -------------------------------------------------------------------------------------------

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if(!flag_cantAct) {
            moveVelocity = ((xzInput.x * transform.right) + (xzInput.z * transform.forward)) * moveSpeed;
            controller.Move(moveVelocity * Time.deltaTime);
        }
    }

    // -------------------------------------------------------------------------------------------

    public void Move(InputAction.CallbackContext value)
    {
        Vector2 inputValue = value.ReadValue<Vector2>();
        xzInput = new Vector3(inputValue.x, 0f, inputValue.y);
        //Debug.Log(value.ReadValue<Vector2>());
    }

    public void Teleport(Transform other, Vector3 offset = default)
    {
        // TODO: Swap teleport player and other transform
        Debug.Log("Teleport to " + other.gameObject.name + " at " + other.position + offset, other.gameObject);
    }
}