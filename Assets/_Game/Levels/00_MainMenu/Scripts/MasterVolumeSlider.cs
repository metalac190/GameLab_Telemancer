using UnityEngine;

public class MasterVolume : OptionSlider
{
    public override void SaveValue(int n)
    {
        PlayerPrefs.SetInt("MasterVolume", n);
    }
}