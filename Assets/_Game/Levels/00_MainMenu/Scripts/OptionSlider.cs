using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Abstract class for OptionsMenu items that use slider and (maybe) also text input
/// </summary>
public abstract class OptionSlider : MonoBehaviour
{
    [SerializeField] private int _value;
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _text;

    public void Start()
    {
        _slider.onValueChanged.AddListener(delegate {SetValue((int)_slider.value);});
    }

    public abstract void SaveValue(int n);

    public virtual void SetValue(int n)
    {
        _text.text = n + "%";
        SaveValue(n);
    }

    /* Overloaded to convert string input into int */
    public virtual void SetValue(string s)
    {
        int n = int.Parse(s);
        SetValue(n);
    }

    public void SetText(string s)
    {
        _text.text = s;
    }
}