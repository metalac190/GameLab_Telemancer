using UnityEngine;

namespace Mechanics.Player
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        [SerializeField] private PlayerFeedback _playerFeedback = null;

        public void OnPoint()
        {
            if (_playerFeedback == null) return;
            _playerFeedback.OnAnimationPoint();
        }

        public void OnSnap()
        {
            if (_playerFeedback == null) return;
            _playerFeedback.OnAnimationSnap();
        }
    }
}
