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

        public void Play(Vector3 position)
        {
            SetVariationValues();

            if (Clip == null)
            {
                Debug.LogWarning("SFXLoop.Play: No Clips Specified");
                return;
            }

            SFXManager.Instance.PlayLoop(this, position);
        }

        public void Stop(AudioSource audioSource)
        {
            audioSource.Stop();
        }
    }
}
