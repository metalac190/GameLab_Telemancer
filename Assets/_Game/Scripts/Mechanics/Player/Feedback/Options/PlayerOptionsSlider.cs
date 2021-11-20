using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mechanics.Player.Feedback.Options
{
    public class PlayerOptionsSlider : MonoBehaviour
    {
        [SerializeField] private Slider _slider = null;
        [SerializeField] private Text _valueText = null;

        public event Action<float> OnSetValue = delegate { };

        private float _value;

        private void Awake()
        {
            _value = _slider.value;
        }

        private void Start()
        {
            _slider.onValueChanged.AddListener(SetValue);
        }

        private void SetValue(float value)
        {
            _value = value;
            OnSetValue.Invoke(value);
            _valueText.text = Mathf.FloorToInt(value * 100) + "%";
        }

        public void Refresh()
        {
            OnSetValue?.Invoke(_value);
        }
    }
}