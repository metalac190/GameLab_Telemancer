using UnityEngine;

public class MasterVolumeSlider : OptionSlider
{
    public override void SaveValue(int n)
    {
        PlayerPrefs.SetInt("MasterVolume", n);
    }
}