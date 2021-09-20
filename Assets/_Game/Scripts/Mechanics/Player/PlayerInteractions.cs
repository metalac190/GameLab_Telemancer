using UnityEngine;

namespace Mechanics.Player
{
    // The Player Interaction Controller, allows the player to interact directly with objects
    // Should be on the camera transform or camera parent transform
    public class PlayerInteractions : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _maxInteractDistance = 5;
        [SerializeField] private LayerMask _interactionMask = 1;
        [Header("References")]
        [SerializeField] private PlayerAnimator _playerAnimator;

        #region NullCheck

        private bool _missingAnimator;

        private void AnimatorNullCheck()
        {
            if (_playerAnimator == null) {
                _playerAnimator = transform.parent != null ? transform.parent.GetComponentInChildren<PlayerAnimator>() : GetComponent<PlayerAnimator>();
                if (_playerAnimator == null) {
                    _missingAnimator = true;
                    Debug.LogWarning("Cannot find the Player Animator for the Player Casting Script", gameObject);
                }
            }
        }

        #endregion

        #region Unity Fucntions

        private void OnEnable()
        {
            AnimatorNullCheck();
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Public Functions

        public void Interact()
        {
            GameObject interactionObject = GetRaycast();
            if (interactionObject == null) return;

            Debug.Log("Interact with: " + gameObject);

            // Get Component for Toad {
            if (!_missingAnimator) {
                // _playerAnimator.OnPetToad();
            }
            // }
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Private Functions

        private GameObject GetRaycast()
        {
            Ray ray = new Ray(transform.position, transform.eulerAngles);

            Physics.Raycast(ray, out var hit, _maxInteractDistance, _interactionMask);
            return hit.collider != null ? hit.collider.gameObject : null;
        }

        #endregion
    }
}