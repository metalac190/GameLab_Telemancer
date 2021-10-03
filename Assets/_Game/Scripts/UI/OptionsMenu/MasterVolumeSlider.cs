using UnityEngine;

public class MasterVolumeSlider : OptionSlider
{
    protected override void SaveValue(int n)
    {
        PlayerPrefs.SetInt("MasterVolume", n);
    }
}