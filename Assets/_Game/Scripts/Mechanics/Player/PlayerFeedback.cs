using Mechanics.WarpBolt;
using UnityEngine;

namespace Mechanics.Player
{
    // The Player Feedback Script, contains all sound or visual effects that the player directly causes
    // Used by anything connected to the player
    public class PlayerFeedback : MonoBehaviour
    {
        [SerializeField] private Temp_UIColorChanger _hudLookAtInteractableState = null;

        public void OnHudColorChange(int type)
        {
            if (_hudLookAtInteractableState == null) return;

            // Looking at Interactable is either -1, 0, or 1, for Null, Object, and Interactable, respectfully
            switch (type) {
                case 1:
                    _hudLookAtInteractableState.SetColor(Color.cyan);
                    break;
                case 0:
                    _hudLookAtInteractableState.SetColor(Color.white);
                    break;
                default:
                    _hudLookAtInteractableState.SetColor(Color.white);
                    break;
            }
        }
    }
}