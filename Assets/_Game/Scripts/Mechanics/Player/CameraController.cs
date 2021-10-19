using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    private PlayerController pc;

    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float maxLookDown = 25f;
    [SerializeField] private float maxLookUp = 60f;
    public float sensitivity = 1;

    private float xRotation; // Rotation around x-axis (vertical)

#pragma warning restore 0649

    // -------------------------------------------------------------------------------------------

    private void Awake() {
        pc = GetComponent<PlayerController>();
        UpdateSettings();
        UIEvents.current.OnSaveCurrentSettings += UpdateSettings;
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // -------------------------------------------------------------------------------------------

    public void MoveCamera(InputAction.CallbackContext value) {
        if(!pc.flag_cantAct) {
            Vector2 mouse = sensitivity * Time.deltaTime * value.ReadValue<Vector2>();
            transform.Rotate(Vector3.up * mouse.x);

            xRotation = Mathf.Clamp(xRotation - mouse.y, -maxLookUp, maxLookDown);
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    public void UpdateSettings() {
        float newFov = PlayerPrefs.GetFloat(OptionSlider.PlayerPrefKey.Fov.ToString());
        mainCamera.fieldOfView = (newFov != 0) ? newFov : 60;

        float newSensitivity = PlayerPrefs.GetFloat(OptionSlider.PlayerPrefKey.Sensitivity.ToString());
        sensitivity = (newSensitivity != 0) ? newSensitivity : 10;
    }

}