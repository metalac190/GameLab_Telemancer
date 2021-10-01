using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    private CharacterController controller;
    private PlayerGroundDetection groundDetector;
    private CapsuleCollider groundDetectorCollision;

    [Header("Horizontal Movement")]
    [Range(0,50)] public float moveSpeed;

    private Vector3 moveVelocity;
    private Vector3 xzInput;

    [Header("Vertical Movement")]
    [SerializeField] [Range(0,20)] private float jumpForce;
    [SerializeField] [Range(0,50)] private float risingGravity, fallingGravity;
    [SerializeField] [Range(0,1)] private float floatTime;
    private bool floating;
    private bool flag_jump, flag_canFloat;

    [Header("General Control")]
    public UnityEvent OnPlayerDeath;
    public bool grounded;
    public bool flag_cantAct;

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
        groundDetector = GetComponentInChildren<PlayerGroundDetection>();
        groundDetectorCollision = groundDetector.GetComponent<CapsuleCollider>();
        OnPlayerDeath.AddListener(() => {
            flag_cantAct = true;
        });
    }

    private void FixedUpdate() {
        // Movement
        if(!flag_cantAct) {
            // XZ Axis
            moveVelocity = (((xzInput.x * transform.right) + (xzInput.z * transform.forward)) * moveSpeed) + (moveVelocity.y * transform.up);

            #region Y Axis
            if(flag_jump) { // Jump
                moveVelocity.y = jumpForce;
                flag_jump = false;
                flag_canFloat = true;

            } else if((grounded && moveVelocity.y < 0) || floating) { // Grounded or floating - stop gravity
                moveVelocity.y = 0;
                flag_canFloat = false;

            } else if(!grounded && flag_canFloat && Mathf.Abs(moveVelocity.y) <= 0.05f) { // Peak of jump - float (can only float once per jump)
                StartCoroutine(Float());

            } else { // Gravity
                moveVelocity.y -= (moveVelocity.y > 0 ? risingGravity : fallingGravity) * Time.fixedDeltaTime;
            }
            #endregion

            // Apply
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
        // TODO: Swap teleport player and other transform
        Vector3 oldPlayerPos = transform.position;

        controller.enabled = false;
        transform.position = other.position + offset;
        other.position = oldPlayerPos;
        controller.enabled = true;

        Debug.Log("Teleport to " + other.gameObject.name + " at " + other.position + offset, other.gameObject);
    }

    public void TeleportToPosition(Vector3 other, Vector3 offset = default) {
        controller.enabled = false;
        transform.position = other + offset;
        controller.enabled = true;

        Debug.Log("Teleport to raw position " + other);
    }

    private IEnumerator Float() {
        if(!floating) {
            flag_canFloat = false;
            floating = true;
            if(floatTime > 0)
                yield return new WaitForSeconds(floatTime);
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