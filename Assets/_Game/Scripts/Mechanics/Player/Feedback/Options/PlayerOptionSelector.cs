using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mechanics.Player.Feedback.Options
{
    public class PlayerOptionSelector : MonoBehaviour
    {
        [SerializeField] private Button _offButton = null;
        [SerializeField] private Button _onButton = null;

        public event Action<bool> OnSelect = delegate { };

        private bool _on;

        private void Start()
        {
            _offButton.onClick.AddListener(delegate { SelectItem(false); });
            _onButton.onClick.AddListener(delegate { SelectItem(true); });
        }

        public void SelectItem(bool on)
        {
            _offButton.interactable = on;
            _onButton.interactable = !on;
            OnSelect?.Invoke(on);
        }

        public void SetCurrentItem(bool on)
        {
            _offButton.interactable = on;
            _onButton.interactable = !on;
        }
    }
}