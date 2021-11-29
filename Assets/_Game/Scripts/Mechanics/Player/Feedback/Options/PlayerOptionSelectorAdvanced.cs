using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mechanics.Player.Feedback.Options
{
    public class PlayerOptionSelectorAdvanced : MonoBehaviour
    {
        [SerializeField] private Button _offButton = null;
        [SerializeField] private Button _defaultButton = null;
        [SerializeField] private Button _onButton = null;

        public event Action<int> OnSelect = delegate { };

        private int _state;

        private void Start()
        {
            _offButton.onClick.AddListener(delegate { SelectItem(-1); });
            _defaultButton.onClick.AddListener(delegate { SelectItem(0); });
            _onButton.onClick.AddListener(delegate { SelectItem(1); });
        }

        public void SelectItem(int state)
        {
            _state = state;
            _offButton.interactable = state >= 0;
            _defaultButton.interactable = state != 0;
            _onButton.interactable = state <= 0;
            OnSelect?.Invoke(state);
        }

        public void Refresh()
        {
            OnSelect?.Invoke(_state);
        }
    }
}