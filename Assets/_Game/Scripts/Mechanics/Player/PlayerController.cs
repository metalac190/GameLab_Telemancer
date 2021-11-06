using System.Collections;
using System.Collections.Generic;
using Mechanics.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField
    
    // --- Components

    private CharacterController controller;
    private CameraController cameraController;

    // --- Horizontal Movement

    private Vector3 moveVelocity;
    private Vector3 xzInput;

    // --- Vertical Movement

    private int jumpBufferFrameCount; // The amount of frames remaining in a jump input buffer
    private bool floating;
    private bool flag_jump, flag_canFloat;

    // ---

    [Header("General Control")]
    public UnityEvent OnTeleport;
    public PlayerFeedback playerFeedback;
    public bool grounded;
    [SerializeField] private bool coyoteTimeActive;
    public bool walking;
    public bool flag_cantAct;

    private bool recentlyTeleported = false;
    private bool wasGrounded = false;

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
        cameraController = GetComponent<CameraController>();
        OnTeleport.AddListener(() => { 
            moveVelocity = Vector3.zero;
            StartCoroutine(RecentlyTeleportTimer());
        });
    }

    private void FixedUpdate() {
        // Movement
        if(!flag_cantAct && controller.enabled) {
            #region XZ Plane
            Vector3 inputToMovement = ((xzInput.x * transform.right) + (xzInput.z * transform.forward)).normalized;
            float upVelocity = moveVelocity.y;
            moveVelocity.y = 0;

            if(grounded) { // Ground movement
                if(moveVelocity.magnitude > PlayerState.Settings.MoveSpeed) {
                    moveVelocity = inputToMovement * 
                        Mathf.Max(moveVelocity.magnitude - (PlayerState.Settings.AirAcceleration * Time.fixedDeltaTime), PlayerState.Settings.MoveSpeed);
                } else
                    moveVelocity = inputToMovement * PlayerState.Settings.MoveSpeed;
            } else { // Air movement
                moveVelocity = ApplyAirAcceleration(moveVelocity, inputToMovement);
            }

            moveVelocity += upVelocity * Vector3.up;
            #endregion

            // -----

            #region Y Axis
            // Coyote Time
            if(!coyoteTimeActive && wasGrounded && !grounded && !recentlyTeleported)
                StartCoroutine(CoyoteTime());
            wasGrounded = grounded;

            // Jumping/Gravity
            if(flag_jump || (grounded && jumpBufferFrameCount > 0)) { // Jump
                moveVelocity.y = PlayerState.Settings.JumpForce;
                playerFeedback.OnPlayerJump();
                flag_jump = false;
                flag_canFloat = true;
                jumpBufferFrameCount = 0;

            } else if((grounded && moveVelocity.y < 0) || floating) { // Grounded or floating - stop gravity
                moveVelocity.y = 0;
                flag_canFloat = false;

            } else if(!grounded && flag_canFloat && Mathf.Abs(moveVelocity.y) <= 0.05f) { // Peak of jump - float (can only float once per jump)
                StartCoroutine(Float());

            } else { // Gravity
                moveVelocity.y -= (moveVelocity.y > 0 ? PlayerState.Settings.RisingGravity : PlayerState.Settings.FallingGravity) * Time.fixedDeltaTime;
                jumpBufferFrameCount = Mathf.Clamp(jumpBufferFrameCount - 1, 0, PlayerState.Settings.JumpBuffer);
            }
            #endregion

            // -----

            // Is Walking
            walking = grounded && moveVelocity.magnitude > 0.5f;

            // Apply
            playerFeedback.SetPlayerVelocity(moveVelocity, grounded, walking);
            if (controller.enabled) {
                controller.Move(moveVelocity * Time.fixedDeltaTime);
            }
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
            if(grounded || coyoteTimeActive || infiniteJumps)
                flag_jump = true;
            else
                jumpBufferFrameCount = PlayerState.Settings.JumpBuffer;
        }
    }

    #endregion

    // -------------------------------------------------------------------------------------------

    #region Teleport

    public void Teleport(Transform other, Vector3 offset = default) {
        BoxCollider collider = other.GetComponent<BoxCollider>();
        Vector3 oldPlayerPos, newPlayerPos, colliderOffset = Vector3.zero;
        if(collider) {
            oldPlayerPos = controller.bounds.min + new Vector3(controller.bounds.extents.x, 0f, controller.bounds.extents.z);
            newPlayerPos = collider.bounds.min + new Vector3(collider.bounds.extents.x, 0f, collider.bounds.extents.z);
            colliderOffset = other.position - newPlayerPos;
            newPlayerPos += offset;
        } else {
            oldPlayerPos = transform.position;
            newPlayerPos = other.position + offset;
        }
        StartCoroutine(TeleportLerp(oldPlayerPos, newPlayerPos, true, other, colliderOffset));

        //Debug.Log("Teleport to " + other.gameObject.name + " at " + (other.position + offset), other.gameObject);
    }

    public void TeleportToPosition(Vector3 other, Vector3 offset = default) {
        StartCoroutine(TeleportLerp(transform.position, other + offset, true));

        //Debug.Log("Teleport to raw position " + other);
    }

    private IEnumerator TeleportLerp(Vector3 startPosition, Vector3 endPosition, bool lerp = false, Transform otherObj = null, Vector3 otherObjOffset = default) {
        controller.enabled = false;
        OnTeleport.Invoke();

        bool otherObjectMoved = false;
        if(lerp) {
            float fraction, originalFov = cameraController.FOV, maxFov = cameraController.FOV + PlayerState.Settings.TeleportFovIncrease;

            for(float i = 0; i < PlayerState.Settings.TeleportTime; i += Time.deltaTime) {
                fraction = i / PlayerState.Settings.TeleportTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, fraction); // Move player
                if(!otherObjectMoved && otherObj && fraction > 0.5f) { // Teleport other object at mid-way point
                    otherObj.position = startPosition + otherObjOffset;
                    otherObjectMoved = true;
                }

                // FOV control
                if(PlayerState.Settings.TeleportFovIncrease > 0) {
                    if(fraction < PlayerState.Settings.TeleportFovMaxPoint) {
                        fraction /= PlayerState.Settings.TeleportFovMaxPoint;
                        cameraController.FOV = originalFov + (PlayerState.Settings.TeleportFovIncrease * fraction);
                    } else {
                        fraction = (fraction - PlayerState.Settings.TeleportFovMaxPoint) / (1 - PlayerState.Settings.TeleportFovMaxPoint);
                        cameraController.FOV = maxFov - (PlayerState.Settings.TeleportFovIncrease * fraction);
                    }
                }

                yield return null;
            }
            cameraController.FOV = originalFov;
        }
        if(!otherObjectMoved && otherObj) { // Still swap objects if no lerp & applicable or if failed to swap earlier
            otherObj.position = startPosition + otherObjOffset;
        }

        // End of teleport
        transform.position = endPosition;
        yield return null;
        controller.enabled = true;
    }

    /// <summary>
    /// Timer to mark when if player had recently teleported
    /// </summary>
    private IEnumerator RecentlyTeleportTimer() {
        recentlyTeleported = true;
        for(float i = 0; i <= 0.1f; i += Time.deltaTime) {
            // Check for break early
            if(!recentlyTeleported)
                break;
            yield return null;
        }
        recentlyTeleported = false;
    }

    #endregion Teleport

    // -------------------------------------------------------------------------------------------

    #region Movement

    /// <summary>
    /// Applies air acceleration to given Vector3
    /// </summary>
    /// <param name="moveVelocity">Player's previous movement speed in the XZ plane, before acceleration</param>
    /// <param name="accelDir">Direction player is attempting to accelerate in (input direction)</param>
    /// <returns></returns>
    private Vector3 ApplyAirAcceleration(Vector3 moveVelocity, Vector3 accelDir) {
        moveVelocity += PlayerState.Settings.AirAcceleration * Time.fixedDeltaTime * accelDir;
        moveVelocity = moveVelocity.normalized * Mathf.Clamp(moveVelocity.magnitude, 0, PlayerState.Settings.MoveSpeed);
        return moveVelocity;
    }

    private IEnumerator CoyoteTime() {
        coyoteTimeActive = true;
        for(float i = 0; i <= PlayerState.Settings.CoyoteJumpTime; i += Time.deltaTime) {
            // Check for break early
            if(!coyoteTimeActive || grounded || controller.velocity.y > 0)
                break;

            yield return null;
        }
        coyoteTimeActive = false;

    }

    private IEnumerator Float() {
        if(!floating) {
            flag_canFloat = false;
            floating = true;
            if(PlayerState.Settings.FloatTime > 0)
                yield return new WaitForSeconds(PlayerState.Settings.FloatTime);
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