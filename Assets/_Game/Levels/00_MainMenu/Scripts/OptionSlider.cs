using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Abstract class for OptionsMenu items that use slider
/// </summary>
public abstract class OptionSlider : MonoBehaviour
{
    [SerializeField] private int _value;
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _text;
    //[SerializeField] protected string playerPrefKey;

    public enum PlayerPrefKey
    {
        MasterVolume,
        SfxVolume,
        MusicVolume,
        Fov,
        Sensitiviy
    }

    [SerializeField] private PlayerPrefKey prefKey;

    public void Start()
    {
        MenuEvents.current.ONReloadSettings += LoadValue;
        _slider.onValueChanged.AddListener(delegate {SetValue((int)_slider.value);});
    }

    protected virtual void SaveValue(int n)
    {
        PlayerPrefs.SetFloat(prefKey.ToString(), n);
    }

    protected virtual void LoadValue()
    {
        float val = PlayerPrefs.GetFloat(prefKey.ToString());
        SetText(val + ""); // TODO: look into making SetText the sole way to update the stuff...
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