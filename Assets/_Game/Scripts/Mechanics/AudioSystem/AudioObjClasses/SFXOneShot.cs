using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    [CreateAssetMenu(menuName = "AudioSystem/SFX OneShot", fileName = "SFX_OS_")]
    public class SFXOneShot : SFXEvent
    {
        public void PlayOneShot(Vector3 position)
        {
            SetVariationValues();

            if (Clip == null)
            {
                Debug.LogWarning("SFXOneShot.PlayOneShot: No Clips Specified");
                return;
            }

            SFXManager.Instance.PlayOneShot(this, position);
        }

        public void PlayOneShot2D()
        {
            SetVariationValues();

            if (Clip == null)
            {
                Debug.LogWarning("SFXOneShot.PlayOneShot: No Clips Specified");
                return;
            }

            SFXManager.Instance.PlayOneShot(this, Vector3.zero);
        }

        public void Preview(AudioSource source)
        {
            SetVariationValues();

            if (Clip == null) return;

            source.clip = Clip;
            source.outputAudioMixerGroup = Mixer;
            source.priority = Priority;
            source.volume = Volume;
            source.pitch = Pitch;
            source.panStereo = StereoPan;
            source.spatialBlend = SpatialBlend;

            source.Play();
        }
    }
}