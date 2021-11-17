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

        private void Start()
        {
            int index = SceneManager.GetActiveScene().buildIndex - _levelStartOffset;
            for (int x = 0; x < _buttons.Length; x++) {
                var x1 = x;
                _buttons[x1].onClick.AddListener(delegate { SelectItem(x1); });
                _buttons[x1].interactable = x1 != index;
            }
        }

        public void SelectItem(int value)
        {
            OnChangeLevel?.Invoke(_levelStartOffset + value);
        }
    }
}