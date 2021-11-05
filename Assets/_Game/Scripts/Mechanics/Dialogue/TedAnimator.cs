using UnityEngine;

namespace Mechanics.Dialogue
{
    public class TedAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _tedAnimator;
        [SerializeField] private string _inConversationString = "InConversation";

        public void SetTalking(bool talking)
        {
            if (_tedAnimator == null) return;
            _tedAnimator.SetBool(_inConversationString, talking);
        }
    }
}