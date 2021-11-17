using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mechanics.Player.Feedback.Options
{
    public class PlayerLevelSelector : MonoBehaviour
    {
        [SerializeField] private int _levelStartOffset = 1;
        [SerializeField] private Button[] _buttons = null;

        public event Action<int> OnChangeLevel = delegate { };

        private int _currentActive = -1;

        private void Start()
        {
            for (int i = 0; i < _buttons.Length; i++) {
                var temp = i;
                _buttons[temp].onClick.AddListener(delegate { SelectItem(temp); });
            }
        }

        private void OnEnable()
        {
            int current = SceneManager.GetActiveScene().buildIndex - _levelStartOffset;
            if (current == _currentActive) return;
            _currentActive = current;
            for (int i = 0; i < _buttons.Length; i++) {
                _buttons[i].interactable = i != _currentActive;
            }
        }

        public void SelectItem(int value)
        {
            OnChangeLevel?.Invoke(_levelStartOffset + value);
        }
    }
}