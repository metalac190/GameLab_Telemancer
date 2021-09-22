using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    public abstract class SFXEvent : ScriptableObject
    {
        [Header("General Settings (Subject to Change)")]
        [SerializeField] AudioClip[] possibleClips = new AudioClip[0];
        [SerializeField] AudioMixerGroup mixer = null;
        [Space(15)]

        [Range(0, 128)]
        [SerializeField]
        int priority = 128;

        [SerializeField]
        [MinMaxRange(0, 1)]
        RangedFloat volume = new RangedFloat(.8f, .8f);

        [SerializeField]
        [MinMaxRange(0, 2)]
        RangedFloat pitch = new RangedFloat(.95f, 1.05f);

        [SerializeField]
        [Range(-1, 1)]
        private float stereoPan = 0;

        [SerializeField]
        [Range(0, 1)]
        float spatialBlend = 0;

        [Header("3D Settings")]
        [SerializeField] float attenuationMin = 1;
        [SerializeField] float attenuationMax = 500;

        int clipIndex = 0;

        public AudioClip Clip => possibleClips[clipIndex];
        public AudioMixerGroup Mixer => mixer;

        public int Priority => priority;
        public float Volume { get; private set; }
        public float Pitch { get; private set; }
        public float StereoPan => stereoPan;
        public float SpatialBlend => spatialBlend;
        public float AttenuationMin => attenuationMin;
        public float AttenuationMax => attenuationMax;

        protected void SetVariationValues()
        {
            clipIndex = Random.Range(0, possibleClips.Length);
            Volume = Random.Range(volume.MinValue, volume.MaxValue);
            Pitch = Random.Range(pitch.MinValue, pitch.MaxValue);
        }
    }
}
