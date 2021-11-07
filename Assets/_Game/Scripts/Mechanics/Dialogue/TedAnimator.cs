using UnityEngine;

namespace Mechanics.Dialogue
{
    public class TedAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _tedAnimator = null;
        [SerializeField] private string _inConversationString = "InConversation";

        public void SetTalking(bool talking)
        {
            _tedAnimator.SetBool(_inConversationString, talking);
        }
    }
}