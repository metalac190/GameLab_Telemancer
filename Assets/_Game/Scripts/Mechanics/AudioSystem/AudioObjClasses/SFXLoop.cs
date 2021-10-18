using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    [CreateAssetMenu(menuName = "AudioSystem/SFX Looped", fileName = "SFX_LP_")]
    public class SFXLoop : SFXEvent
    {
        [HideInInspector] public int NumCycles = 0;
        [HideInInspector] public bool IsLoopedInfinitely = true;
        [HideInInspector] public bool FiniteLoopingEnabled = false;

        public AudioSource Play(Vector3 position)
        {
            SetVariationValues();

            if (Clip == null)
            {
                Debug.LogWarning("SFXLoop.Play: No Clips Specified");
                return null;
            }

            return SFXManager.Instance.PlayLoop(this, position);
        }

        public void Stop(AudioSource audioSource)
        {
            audioSource.Stop();
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

            source.loop = true;
            source.Play();
        }

        public void StopPreview(AudioSource source)
        {
            source.loop = false;
        }
    }
}
