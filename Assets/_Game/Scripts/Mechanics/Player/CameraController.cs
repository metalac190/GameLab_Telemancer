using System.Collections;
using System.Collections.Generic;
using Mechanics.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {

    private PlayerController pc;

    [SerializeField] private float sensitivity = 10;

    [SerializeField] private Transform cameraHolder = null;
    [SerializeField] private Transform viewBobber = null;
    [SerializeField] private Camera mainCamera = null;

    private float xRotation; // Rotation around x-axis (vertical)

    // -------------------------------------------------------------------------------------------

    private void Awake() {
        pc = GetComponent<PlayerController>();
        UpdateSettings();
        UIEvents.current.OnSaveCurrentSettings += UpdateSettings;
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if(!pc.flag_cantAct) {
            Vector2 mouse = sensitivity * Time.deltaTime * value.ReadValue<Vector2>();
            transform.Rotate(Vector3.up * mouse.x);

            xRotation = Mathf.Clamp(xRotation - mouse.y, -PlayerState.Settings.MaxLookUp, PlayerState.Settings.MaxLookDown);
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    public void UpdateSettings() {
        float newFov = PlayerPrefs.GetFloat(OptionSlider.PlayerPrefKey.Fov.ToString());
        FOV = (newFov != 0) ? newFov : 60;

        float newSensitivity = PlayerPrefs.GetFloat(OptionSlider.PlayerPrefKey.Sensitivity.ToString());
        sensitivity = (newSensitivity != 0) ? newSensitivity : 10;
    }

    // -------------------------------------------------------------------------------------------

    private IEnumerator ViewBobbing() {
        throw new System.NotImplementedException();
    }

}