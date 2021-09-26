using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

namespace Mechanics.Player
{
    /// Summary:
    /// The Player Feedback Script, contains all sound or visual effects that the player directly causes
    /// Used by anything connected to the player
    public class PlayerFeedback : MonoBehaviour
    {
        // @Brett should probably take over audio and hud Feedback
        // This is more for debugging and rough feedback

        [Header("HUD Crosshair Display")]
        [SerializeField] private Text _hudLookAtInteractable = null;

        [Header("HUD Abilities Display")]
        [SerializeField] private Image _castImage = null;
        [SerializeField] private Image _warpImage = null;
        [SerializeField] private Image _residueImage = null;
        [SerializeField] private float _inputFlashTime = 0.1f;

        [SerializeField] private Color _disabledColor = new Color(1, 1, 1, 36f / 255f);
        [SerializeField] private Color _normalColor = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Color _usedColor = new Color(0.5f, 0.75f, 0.5f, 0.75f);
        [SerializeField] private Color _readyToUseColor = new Color(0.8f, 0.7f, 0.4f, 0.6f);
        [SerializeField] private Color _failedColor = new Color(0.75f, 0.5f, 0.5f, 0.5f);

        [Header("VFX Flash on Cast")]
        [SerializeField] private VfxController _castFlash;
        [SerializeField] private Transform _whereToFlash = null;

        private void Awake()
        {
            // Ensure that VFX is in scene (allows for prefab reference)
            if (_castFlash != null && !_castFlash.gameObject.activeInHierarchy) {
                Transform location = _whereToFlash != null ? _whereToFlash : transform;
                _castFlash = Instantiate(_castFlash, location);
            }
        }

        public void OnUpdateUnlockedAbilities(bool warpAbility, bool residueAbility)
        {
            if (_warpImage != null) {
                _warpImage.color = warpAbility ? _normalColor : _disabledColor;
            }
            if (_residueImage != null) {
                _residueImage.color = residueAbility ? _normalColor : _disabledColor;
            }
        }

        public void OnPrepareToCast(bool wasSuccessful = true)
        {
            if (_castImage != null) {
                StartCoroutine(InputDebug(_castImage, wasSuccessful));
            }
        }

        public void OnCastBolt()
        {
            if (_castFlash != null) {
                _castFlash.Play();
            }
        }

        public void OnWarpReady(bool ready = true)
        {
            if (_warpImage != null) {
                _warpImage.color = ready ? _readyToUseColor : _normalColor;
            }
        }

        public void OnActivateWarp(bool wasSuccessful = true)
        {
            if (_warpImage != null) {
                StartCoroutine(InputDebug(_warpImage, wasSuccessful));
            }
        }

        public void OnResidueReady()
        {
            if (_residueImage != null) {
                _residueImage.color = _readyToUseColor;
            }
        }

        public void OnActivateResidue(bool wasSuccessful = true)
        {
            if (_residueImage != null) {
                StartCoroutine(InputDebug(_residueImage, wasSuccessful));
            }
        }

        public void OnHudColorChange(InteractableEnums type)
        {
            if (_hudLookAtInteractable == null) return;

            // Looking at Interactable is either -1, 0, or 1, for Null, Object, and Interactable, respectfully
            switch (type) {
                case InteractableEnums.WarpInteractable:
                    _hudLookAtInteractable.color = Color.cyan;
                    break;
                case InteractableEnums.PlayerInteractable:
                    _hudLookAtInteractable.color = Color.green;
                    break;
                case InteractableEnums.Object:
                case InteractableEnums.Null:
                    _hudLookAtInteractable.color = Color.white;
                    break;
            }
        }

        private IEnumerator InputDebug(Image image, bool successful)
        {
            image.color = successful ? _usedColor : _failedColor;
            yield return new WaitForSecondsRealtime(_inputFlashTime);
            image.color = _normalColor;
        }
    }
}