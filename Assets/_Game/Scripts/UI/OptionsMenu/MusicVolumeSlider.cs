using UnityEngine;

public class MusicVolumeSlider : OptionSlider
{
    protected override void SaveValue(float n)
    {
        PlayerPrefs.SetFloat("MusicVolume", n);
    }
}