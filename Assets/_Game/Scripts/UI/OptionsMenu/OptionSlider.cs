using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Abstract class for OptionsMenu items that use slider
/// </summary>
public abstract class OptionSlider : MonoBehaviour
{
    public enum PlayerPrefKey
    {
        MasterVolume,
        SfxVolume,
        MusicVolume,
        Fov,
        Sensitivity
    }

    // Have PlayerPrefKey selected via dropdown menu in the inspector
    [SerializeField] private PlayerPrefKey prefKey = PlayerPrefKey.MasterVolume;
    
    [SerializeField] private int _value;
    [SerializeField] private Slider _slider = null;
    [SerializeField] private Text _text = null;
    
    public void Start()
    {
        UIEvents.current.OnReloadSettings += LoadValue;
        _slider.onValueChanged.AddListener(delegate {SetValue((int)_slider.value);});
        
        LoadValue();
    }

    public void OnBecameVisible()
    {
        LoadValue();
    }

    protected virtual void SaveValue(int n)
    {
        PlayerPrefs.SetFloat(prefKey.ToString(), n);
    }

    protected virtual void LoadValue()
    {
        float val = PlayerPrefs.GetFloat(prefKey.ToString());
        SetText(val + "");
        _slider.value = val;
    }

    protected virtual void SetValue(int n)
    {
        SetText(n + "");
        SaveValue(n);
    }

    protected virtual void SetText(string s)
    {
        _text.text = s;
    }
}