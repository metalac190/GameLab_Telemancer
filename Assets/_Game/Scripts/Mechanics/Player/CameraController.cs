using System.Collections;
using System.Collections.Generic;
using Mechanics.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {

    private PlayerController pc;
    private PlayerInput input;

    [SerializeField] private float sensitivity = 10;
    [SerializeField] [Tooltip("The amount camera movement is multiplied by when using a controller")] private float controllerMultiplier = 10;
    [SerializeField] private Transform cameraHolder = null;
    [SerializeField] private Camera mainCamera = null;

    private Vector2 mouseInput, smoothedInput;
    private float xRotation; // Rotation around x-axis (vertical)

    // ---

    [Header("View Bobbing")]
    [SerializeField] private Transform viewBobber = null;
    private float walkingTime;
    private Vector3 targetCameraPosition;

    // -------------------------------------------------------------------------------------------

    private void Awake() {
        pc = GetComponent<PlayerController>();
        input = GetComponent<PlayerInput>();
        UpdateSettings();
        UIEvents.current.OnSaveCurrentSettings += UpdateSettings;
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // -------------------------------------------------------------------------------------------

    private void Update() {
        #region Camera Rotation
        smoothedInput = sensitivity * Time.deltaTime * (input.currentControlScheme.Equals("Controller") ? controllerMultiplier : 1) * mouseInput;
        transform.Rotate(Vector3.up * smoothedInput.x);

        xRotation = Mathf.Clamp(xRotation - smoothedInput.y, -PlayerState.Settings.MaxLookUp, PlayerState.Settings.MaxLookDown);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        #endregion

        // ---

        #region View Bobbing
        if(!PlayerState.Settings.ViewBobbingEnabled) {
            viewBobber.localPosition = Vector3.zero;
            return;
        }

        if(!pc.walking)
            walkingTime = 0;
        else
            walkingTime += Time.deltaTime;

        // Lerp camera
        targetCameraPosition = viewBobber.InverseTransformPoint(viewBobber.position + GetBobbingPosition(walkingTime));
        viewBobber.localPosition = Vector3.Lerp(viewBobber.localPosition, targetCameraPosition, PlayerState.Settings.ViewBobbingSmoothing);
        if((viewBobber.localPosition - targetCameraPosition).magnitude <= 0.01f) { // Snap to target if close enough
            viewBobber.localPosition = targetCameraPosition;
        }
        #endregion
    }

    // -------------------------------------------------------------------------------------------

    public float FOV {
        get {
            return mainCamera.fieldOfView;
        }
        set {
            // TODO - convert from horizontal to vertical (copy Henry's script)
            mainCamera.fieldOfView = value;
        }
    }

    // -------------------------------------------------------------------------------------------

    public void MoveCamera(InputAction.CallbackContext value) {
        mouseInput = !pc.flag_cantAct ? value.ReadValue<Vector2>() : Vector2.zero;
        /*if(!pc.flag_cantAct) {
            Vector2 mouse = sensitivity * Time.deltaTime * value.ReadValue<Vector2>();
            transform.Rotate(Vector3.up * mouse.x);

            xRotation = Mathf.Clamp(xRotation - mouse.y, -PlayerState.Settings.MaxLookUp, PlayerState.Settings.MaxLookDown);
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }*/
    }

    public void UpdateSettings() {
        float newFov = PlayerPrefs.GetFloat(OptionSlider.PlayerPrefKey.Fov.ToString());
        FOV = (newFov != 0) ? newFov : 60;

        float newSensitivity = PlayerPrefs.GetFloat(OptionSlider.PlayerPrefKey.Sensitivity.ToString());
        sensitivity = (newSensitivity != 0) ? newSensitivity : 10;
    }

    // -------------------------------------------------------------------------------------------

    private Vector3 GetBobbingPosition(float time) {
        Vector3 offset = Vector3.zero;

        if(time > 0) {
            float x = Mathf.Cos(time * PlayerState.Settings.ViewBobbingFrequency) * PlayerState.Settings.ViewBobbingHorizontal;
            float y = Mathf.Sin(time * PlayerState.Settings.ViewBobbingFrequency * 2) * PlayerState.Settings.ViewBobbingVertical;
            offset = (gameObject.transform.right * x) + (cameraHolder.transform.up * y);
        }
        return offset;
    }

}