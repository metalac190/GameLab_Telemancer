using UnityEngine;

public class SfxVolumeSlider : OptionSlider
{
    protected override void SaveValue(float n)
    {
        PlayerPrefs.SetFloat("SfxVolume", n);
        // Play a funny sound effect
    }
}