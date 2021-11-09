using UnityEngine;

namespace Mechanics.Player
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        [SerializeField] private bool _debug = false;
        [SerializeField] private PlayerFeedback _playerFeedback = null;

        public void OnPoint()
        {
            if (_debug) Debug.Log("Point");
            if (_playerFeedback == null) return;
            _playerFeedback.OnAnimationPoint();
        }

        public void OnSnap()
        {
            if (_debug) Debug.Log("Snap");
            if (_playerFeedback == null) return;
            _playerFeedback.OnAnimationSnap();
        }
    }
}
