using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanics.Player
{
    /// Summary:
    /// The Player Interaction Controller, allows the player to interact directly with objects
    /// Should be on the camera transform or camera parent transform
    public class PlayerInteractions : MonoBehaviour
    {
        [SerializeField] private GameSettingsData _settings;
        [Header("References")]
        [SerializeField] private PlayerFeedback _playerFeedback;

        #region Unity Fucntions

        private void OnEnable()
        {
            NullChecks();
        }

        private void Update()
        {
            LookAtInteractables();
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Public Functions

        public void Interact(InputAction.CallbackContext value)
        {
            if (!value.performed) return;

            var hit = GetRaycast(_settings.maxInteractDistance);
            if (hit.collider == null) return;

            hit.collider.gameObject.GetComponent<IPlayerInteractable>()?.OnInteract();

            // Find interactable and interact
            // Play Animations on player
        }

        public void LookAtInteractables()
        {
            var hit = GetRaycast(_settings.maxLookDistance);
            if (hit.collider == null) {
                SetInteractable(InteractableEnums.Null);
                return;
            }
            SetInteractable(InteractableEnums.Object);
            GameObject interactionObject = hit.collider.gameObject;

            //Debug.Log("Interact with: " + interactionObject.name, interactionObject);

            var interactable = interactionObject.GetComponent<IWarpInteractable>();
            if (interactable != null) {
                SetInteractable(InteractableEnums.WarpInteractable);
                return;
            }

            if (hit.distance < _settings.maxInteractDistance) {
                var playerInteractable = interactionObject.GetComponent<IPlayerInteractable>();
                if (playerInteractable != null) {
                    SetInteractable(InteractableEnums.PlayerInteractable);
                    return;
                }
            }
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Private Functions

        private void SetInteractable(InteractableEnums type)
        {
            if (!_missingFeedback) {
                _playerFeedback.OnCrosshairColorChange(type);
            }
        }

        private RaycastHit GetRaycast(float dist)
        {
            Ray ray = new Ray(transform.position, transform.forward);

            Physics.Raycast(ray, out var hit, dist, _settings.lookAtMask);
            return hit;
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region NullCheck

        private void NullChecks()
        {
            GameSettingsNullCheck();
            FeedbackNullCheck();
        }

        private void GameSettingsNullCheck()
        {
            if (_settings == null) {
                _settings = FindObjectOfType<GameSettingsData>();
                if (_settings == null) {
                    _settings = new GameSettingsData();
                }
            }
        }

        private bool _missingFeedback;

        private void FeedbackNullCheck()
        {
            if (_playerFeedback == null) {
                _playerFeedback = transform.parent != null ? transform.parent.GetComponentInChildren<PlayerFeedback>() : GetComponent<PlayerFeedback>();
                if (_playerFeedback == null) {
                    _missingFeedback = true;
                    Debug.LogWarning("Cannot find the Player Feedback for the Player Casting Script", gameObject);
                }
            }
        }

        #endregion
    }
}