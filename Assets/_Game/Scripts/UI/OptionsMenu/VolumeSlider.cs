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
        float temp = (float)n / 1000;
        float volume = Mathf.Log10(temp) * 20; // Convert to dB
        print("n is and db volume is: " + n + " " + volume);
        mixer.audioMixer.SetFloat(parameterName, volume);
        base.SetValue(n);
    }
}