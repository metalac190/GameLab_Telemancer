using Mechanics.WarpBolt;
using UnityEngine;
using UnityEngine.VFX;

namespace Mechanics.Player
{
    // The Player Feedback Script, contains all sound or visual effects that the player directly causes
    // Used by anything connected to the player
    public class PlayerFeedback : MonoBehaviour
    {
        [SerializeField] private Temp_UIColorChanger _hudLookAtInteractableState = null;
        [SerializeField] private VisualEffect _castFlash = null;

        public void OnCastBolt()
        {
            if (_castFlash == null) return;

            _castFlash.Play();
        }

        public void OnHudColorChange(InteractableEnums type)
        {
            if (_hudLookAtInteractableState == null) return;

            // Looking at Interactable is either -1, 0, or 1, for Null, Object, and Interactable, respectfully
            switch (type) {
                case InteractableEnums.WarpInteractable:
                    _hudLookAtInteractableState.SetColor(Color.cyan);
                    break;
                case InteractableEnums.PlayerInteractable:
                    _hudLookAtInteractableState.SetColor(Color.green);
                    break;
                case InteractableEnums.Object:
                case InteractableEnums.Null:
                    _hudLookAtInteractableState.SetColor(Color.white);
                    break;
            }
        }
    }
}