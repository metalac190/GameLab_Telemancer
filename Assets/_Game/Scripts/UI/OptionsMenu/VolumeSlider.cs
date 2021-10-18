using UnityEngine;
using UnityEngine.Audio;

public class VolumeSlider : OptionSlider
{
    [SerializeField] AudioMixerGroup mixer;
    [SerializeField] string parameterName;

     protected override void SetText(string s)
     {
          base.SetText(s + "%");
     }

    protected override void SetValue(int n)
    {
        float volume = n - 80;
        mixer.audioMixer.SetFloat(parameterName, volume);
        base.SetValue(n);
    }
}