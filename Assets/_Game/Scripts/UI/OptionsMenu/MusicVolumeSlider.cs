using UnityEngine;

public class MusicVolumeSlider : OptionSlider
{
    protected override void SaveValue(int n)
    {
        PlayerPrefs.SetInt("MusicVolume", n);
    }
}