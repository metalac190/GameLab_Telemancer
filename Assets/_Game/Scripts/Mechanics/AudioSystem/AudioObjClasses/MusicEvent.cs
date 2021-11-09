using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    public enum LayerType
    {
        Additive, //Combine separate tracks together in sequence
        Single //One at a time, blend or fade to next
    }

    [CreateAssetMenu(menuName = "AudioSystem/MusicEvent", fileName = "MUS_")]
    public class MusicEvent : ScriptableObject
    {
        [Header("General Settings")]
        [SerializeField] AudioClip[] musicLayers = null;

        [Tooltip("Additive = Layers Added Together, " +
            "Single = Layers Play Independently")]
        [Space(15)]
        [SerializeField] LayerType layerType = LayerType.Additive;
        [Space(15)]
        [SerializeField] AudioMixerGroup mixer = null;

        public AudioClip[] MusicLayers => musicLayers;
        public LayerType LayerType => layerType;
        public AudioMixerGroup Mixer => mixer;

        [Header("Volume Control")]
        [Space(15)]
        [Range(0, 1)]
        [SerializeField]
        float tempVolume = 0;

        public float TempVolume => tempVolume;

        [Header("Fade Times (In Seconds)")]
        [Space(15)]
        [SerializeField] float initialFadeInTime = 0;
        [SerializeField] float crossfadeTime = 0;
        [SerializeField] float stopSongFadeOutTime = 0;

        public float InitialFadeInTime => initialFadeInTime;
        public float CrossfadeTime => crossfadeTime;
        public float StopSongFadeOutTime => stopSongFadeOutTime;

        public void Play()
        {
            if (musicLayers == null)
            {
                Debug.LogWarning("MusicEvent.Play(): No music clip specified!");
                return;
            }

            MusicManager.Instance.PlayMusic(this, CrossfadeTime);
        }

        public void Stop()
        {
            if (musicLayers == null)
            {
                Debug.LogWarning("MusicEvent.Stop(): No music clip specified!");
                return;
            }

            MusicManager.Instance.StopMusic();
        }

        public void IncreaseLayerIndex(float fadeTime)
        {
            if (musicLayers == null)
            {
                Debug.LogWarning("MusicEvent.Stop(): No music clip specified!");
                return;
            }

            MusicManager.Instance.IncreaseLayerIndex(fadeTime);
        }

        public void DecreaseLayerIndex(float fadeTime)
        {
            if (musicLayers == null)
            {
                Debug.LogWarning("MusicEvent.Stop(): No music clip specified!");
                return;
            }

            MusicManager.Instance.DecreaseLayerIndex(fadeTime);
        }

        public void SetLayerIndex0(float fadeTime)
        {
            if (musicLayers == null)
            {
                Debug.LogWarning("MusicEvent.Stop(): No music clip specified!");
                return;
            }

            MusicManager.Instance.SetLayerIndex(0, fadeTime);
        }

        public void SetLayerIndex1(float fadeTime)
        {
            if (musicLayers == null)
            {
                Debug.LogWarning("MusicEvent.Stop(): No music clip specified!");
                return;
            }

            MusicManager.Instance.SetLayerIndex(1, fadeTime);
        }

        public void SetLayerIndex2(float fadeTime)
        {
            if (musicLayers == null)
            {
                Debug.LogWarning("MusicEvent.Stop(): No music clip specified!");
                return;
            }

            MusicManager.Instance.SetLayerIndex(2, fadeTime);
        }
    }
}