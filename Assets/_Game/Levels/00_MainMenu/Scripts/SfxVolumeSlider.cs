using UnityEngine;

public class SfxVolumeSlider : OptionSlider
{
    public override void SaveValue(int n)
    {
        PlayerPrefs.SetInt("SfxVolume", n);
        // Play a funny sound effect
    }
}