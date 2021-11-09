using UnityEngine;
using UnityEngine.Audio;

public class VolumeSlider : OptionSlider
{
    [SerializeField] AudioMixerGroup mixer = null;
    [SerializeField] string parameterName = "";

     protected override void SetText(string s)
     {
        float percent = float.Parse(s) / 10f;
        base.SetText(percent + "%");
     }

    protected override void SetValue(int n)
    {
        float volume = -80;
        if (n != 0)
            volume = Mathf.Log10((float)n / 1000) * 20; // Convert to dB

        mixer.audioMixer.SetFloat(parameterName, volume);
        base.SetValue(n);
    }
}