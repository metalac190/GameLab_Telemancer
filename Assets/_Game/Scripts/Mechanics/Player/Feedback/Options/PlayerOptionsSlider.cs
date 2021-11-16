using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mechanics.Player.Feedback.Options
{
    public class PlayerOptionsSlider : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _valueText;

        public event Action<float> OnSetValue = delegate {};

        private void Start()
        {
            _slider.onValueChanged.AddListener(SetValue);
        }

        private void SetValue(float value)
        {
            OnSetValue.Invoke(value);
            _valueText.text = Mathf.FloorToInt(value * 100) + "%";
        }
    }
}
