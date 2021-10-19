using System.Collections;
using System.Collections.Generic;
using Mechanics.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField
    
    [Header("Components")]
    private CharacterController controller;

    // --- Horizontal Movement

    private Vector3 moveVelocity;
    private Vector3 xzInput;

    // --- Vertical Movement

    private bool floating;
    private bool flag_jump, flag_canFloat;

    // ---

    [Header("General Control")]
    public UnityEvent OnTeleport;
    public PlayerFeedback playerFeedback;
    public bool grounded, walking;
    public bool flag_cantAct;

    // ---

    [Header("Debug/Testing")]
    [SerializeField] private bool infiniteJumps;
#if UNITY_EDITOR
    public PlayerDebug playerDebug;
    [System.Serializable] public class PlayerDebug {
        public Transform targetTransform;
        public Vector3 targetPosition, offset;
    }
#endif

#pragma warning restore 0649

    // -------------------------------------------------------------------------------------------

    #region Initialize & Repeating

    private void Awake() {
        controller = GetComponent<CharacterController>();
        OnTeleport.AddListener(() => { moveVelocity = Vector3.zero; });
    }

    private void FixedUpdate() {
        // Movement
        if(!flag_cantAct) {
            #region XZ Plane
            Vector3 inputToMovement = ((xzInput.x * transform.right) + (xzInput.z * transform.forward)).normalized;
            if(grounded) {
                moveVelocity = (inputToMovement * PlayerState.settings.moveSpeed) + (moveVelocity.y * transform.up);
            } else {
                float upVelocity = moveVelocity.y;
                moveVelocity.y = 0;
                moveVelocity += PlayerState.settings.airAcceleration * Time.fixedDeltaTime * inputToMovement;
                moveVelocity = moveVelocity.normalized * Mathf.Clamp(moveVelocity.magnitude, 0, PlayerState.settings.moveSpeed);
                moveVelocity += upVelocity * transform.up;
            }
            #endregion

            // -----

            #region Y Axis
            if(flag_jump) { // Jump
                moveVelocity.y = PlayerState.settings.jumpForce;
                playerFeedback.OnPlayerJump();
                flag_jump = false;
                flag_canFloat = true;

            } else if((grounded && moveVelocity.y < 0) || floating) { // Grounded or floating - stop gravity
                moveVelocity.y = 0;
                flag_canFloat = false;

            } else if(!grounded && flag_canFloat && Mathf.Abs(moveVelocity.y) <= 0.05f) { // Peak of jump - float (can only float once per jump)
                StartCoroutine(Float());

            } else { // Gravity
                moveVelocity.y -= (moveVelocity.y > 0 ? PlayerState.settings.risingGravity : PlayerState.settings.fallingGravity) * Time.fixedDeltaTime;
            }
            #endregion

            // -----

            // Is Walking
            walking = grounded && moveVelocity.magnitude > 0.5f;

            // Apply
            playerFeedback.SetPlayerVelocity(moveVelocity, grounded, walking);
            controller.Move(moveVelocity * Time.fixedDeltaTime);
        }
    }

    #endregion

    // -------------------------------------------------------------------------------------------

    #region Inputs

    public void Move(InputAction.CallbackContext value) {
        Vector2 inputValue = value.ReadValue<Vector2>();
        xzInput = new Vector3(inputValue.x, 0f, inputValue.y);
    }

    public void Jump(InputAction.CallbackContext value) {
        if(value.performed) {
            if(grounded || infiniteJumps)
                flag_jump = true;
        }
    }

    #endregion

    // -------------------------------------------------------------------------------------------

    #region Teleport & Movement

    public void Teleport(Transform other, Vector3 offset = default) {
        StartCoroutine(TeleportWithTransform(other, offset));

        //Debug.Log("Teleport to " + other.gameObject.name + " at " + (other.position + offset), other.gameObject);
    }

    private IEnumerator TeleportWithTransform(Transform other, Vector3 offset) {
        controller.enabled = false;
        OnTeleport.Invoke();

        BoxCollider collider = other.GetComponent<BoxCollider>();
        if(collider) {
            Vector3 oldPlayerPos = controller.bounds.min;
            transform.position = collider.bounds.min + new Vector3(collider.bounds.extents.x, 0f, collider.bounds.extents.z) + offset;
            other.position = oldPlayerPos;
        } else {
            Vector3 oldPlayerPos = transform.position;
            transform.position = other.position + offset;
            other.position = oldPlayerPos;
        }
        yield return null;
        controller.enabled = true;
    }

    public void TeleportToPosition(Vector3 other, Vector3 offset = default) {
        controller.enabled = false;
        OnTeleport.Invoke();
        transform.position = other + offset;
        controller.enabled = true;

        //Debug.Log("Teleport to raw position " + other);
    }

    // -------------------

    private IEnumerator Float() {
        if(!floating) {
            flag_canFloat = false;
            floating = true;
            if(PlayerState.settings.floatTime > 0)
                yield return new WaitForSeconds(PlayerState.settings.floatTime);
            floating = false;
        } else 
            Debug.LogError("Player attempting to float while already floating - something must have went wrong???");
        yield return null;
    }

    #endregion

    // -------------------------------------------------------------------------------------------

    #region Debug

#if UNITY_EDITOR
    public void DebugTeleportWithTransform() {
        if(playerDebug.targetTransform)
            Teleport(playerDebug.targetTransform, playerDebug.offset);
        else
            Debug.Log("Debug transform not given, cannot teleport");
    }

    public void DebugTeleportToTransform() {
        if(playerDebug.targetTransform)
            TeleportToPosition(playerDebug.targetTransform.position, playerDebug.offset);
        else
            Debug.Log("Debug transform not given, cannot teleport");
    }

    public void DebugTeleportToVector3() {
        TeleportToPosition(playerDebug.targetPosition, playerDebug.offset);
    }
#endif

    #endregion

}