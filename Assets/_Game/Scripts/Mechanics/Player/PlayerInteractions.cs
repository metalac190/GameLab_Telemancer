using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanics.Player
{
    /// Summary:
    /// The Player Interaction Controller, allows the player to interact directly with objects
    /// Should be on the camera transform or camera parent transform
    public class PlayerInteractions : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _cameraTransform = null;
        [SerializeField] private PlayerFeedback _playerFeedback;
        private bool _isHovering = false;
        private IHoverInteractable _hoverObj;

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

            var hit = GetRaycast(PlayerState.Settings.MaxInteractDistance);
            if (hit.collider == null) return;

            hit.collider.gameObject.GetComponent<IPlayerInteractable>()?.OnInteract();

            // Find interactable and interact
            // Play Animations on player
        }

        public void LookAtInteractables()
        {
            var hit = GetRaycast(PlayerState.Settings.MaxLookDistance);
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

            if (hit.distance < PlayerState.Settings.MaxInteractDistance) {
                var playerInteractable = interactionObject.GetComponent<IPlayerInteractable>();
                if (playerInteractable != null) {
                    SetInteractable(InteractableEnums.PlayerInteractable);
                    // If object is type Hover, save reference and call function
                    if(interactionObject.GetComponent<IHoverInteractable>() != null && !_isHovering)
                    {
                        _isHovering = true;
                        _hoverObj = interactionObject.GetComponent<IHoverInteractable>();
                        _hoverObj.OnBeginHover();
                    }
                    return;
                }
                else if(_isHovering)
                {
                    _isHovering = false;
                    _hoverObj.OnEndHover();
                }
            }
            else if(_isHovering)
            {
                _isHovering = false;
                _hoverObj.OnEndHover();
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
            Transform start = _cameraTransform != null ? _cameraTransform : transform;

            Ray ray = new Ray(start.position, start.forward);

            Physics.Raycast(ray, out var hit, dist, PlayerState.Settings.LookAtMask, QueryTriggerInteraction.Ignore);
            return hit;
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region NullCheck

        private void NullChecks()
        {
            FeedbackNullCheck();
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