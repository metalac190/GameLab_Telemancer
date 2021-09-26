using UnityEngine;

public class SfxVolumeSlider : OptionSlider
{
    protected override void SaveValue(int n)
    {
        PlayerPrefs.SetInt("SfxVolume", n);
        // Play a funny sound effect
    }
}