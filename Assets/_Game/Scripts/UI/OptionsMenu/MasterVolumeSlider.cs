using UnityEngine;

public class MasterVolumeSlider : OptionSlider
{
    protected override void SaveValue(float n)
    {
        PlayerPrefs.SetFloat("MasterVolume", n);
    }
}