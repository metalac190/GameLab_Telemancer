using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    private PlayerController pc;

    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Camera mainCamera;
    public float sensitivity = 1;

    private float xRotation; // Rotation around x-axis (vertical)

#pragma warning restore 0649

    // -------------------------------------------------------------------------------------------

    private void Awake() {
        pc = GetComponent<PlayerController>();
        UpdateSettings();
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // -------------------------------------------------------------------------------------------

    public void MoveCamera(InputAction.CallbackContext value) {
        if(!pc.flag_cantAct) {
            Vector2 mouse = value.ReadValue<Vector2>() * sensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up * mouse.x);

            xRotation = Mathf.Clamp(xRotation - mouse.y, -90f, 90f);
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    public void UpdateSettings() {
        float newFov = Mathf.Clamp(PlayerPrefs.GetFloat(OptionSlider.PlayerPrefKey.Fov.ToString()), 60, Mathf.Infinity);
        mainCamera.fieldOfView = newFov;
        float newSensitivity = Mathf.Clamp(PlayerPrefs.GetFloat(OptionSlider.PlayerPrefKey.Sensitivity.ToString()), 1, Mathf.Infinity);
        sensitivity = newSensitivity;
    }

}