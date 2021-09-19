using UnityEngine;

public class MusicVolumeSlider : OptionSlider
{
    public override void SaveValue(int n)
    {
        PlayerPrefs.SetInt("MusicVolume", n);
    }
}