using UnityEngine;

namespace Mechanics.Player
{
    // The Player Interaction Controller, allows the player to interact directly with objects
    // Should be on the camera transform or camera parent transform
    public class PlayerInteractions : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _maxLookDistance = 20;
        [SerializeField] private float _maxInteractDistance = 5;
        [SerializeField] private LayerMask _interactionMask = 1;
        [Header("References")]
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private PlayerFeedback _playerFeedback;

        #region NullCheck

        private bool _missingAnimator;

        private void AnimatorNullCheck()
        {
            if (_playerAnimator == null) {
                _playerAnimator = transform.parent != null ? transform.parent.GetComponentInChildren<PlayerAnimator>() : GetComponent<PlayerAnimator>();
                if (_playerAnimator == null) {
                    _missingAnimator = true;
                    Debug.LogWarning("Cannot find the Player Animator for the Player Interactions Script", gameObject);
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

        #region Unity Fucntions

        private void OnEnable()
        {
            AnimatorNullCheck();
            FeedbackNullCheck();
        }

        private void Update()
        {
            LookAtInteractables();
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Public Functions

        public void Interact()
        {
            var hit = GetRaycast(_maxInteractDistance);
            if (hit.collider == null) return;

            // Find interactable and interact
            // Play Animations on player
        }

        public void LookAtInteractables()
        {
            var hit = GetRaycast(_maxLookDistance);
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

            if (hit.distance < _maxInteractDistance) {
                // Find interactable

                //SetInteractable(InteractableEnums.PlayerInteractable);
            }
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Private Functions

        private void SetInteractable(InteractableEnums type)
        {
            if (!_missingFeedback) {
                _playerFeedback.OnHudColorChange(type);
            }
        }

        private RaycastHit GetRaycast(float dist)
        {
            Ray ray = new Ray(transform.position, transform.forward);

            Physics.Raycast(ray, out var hit, dist, _interactionMask);
            return hit;
        }

        #endregion
    }
}