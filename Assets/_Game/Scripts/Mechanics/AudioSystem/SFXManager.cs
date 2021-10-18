using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public class SFXManager : MonoBehaviour
    {
        [SerializeField] int startingOneShotPoolSize = 5;
        [SerializeField] int startingLoopPoolSize = 5;
        SoundPool soundPools;

        private static SFXManager instance;
        public static SFXManager Instance
        {
            get
            {
                // Lazy Instantiation
                if (instance == null)
                {
                    instance = FindObjectOfType<SFXManager>();

                    if (instance == null)
                    {
                        GameObject singletonGO = new GameObject("SFXManager");
                        instance = singletonGO.AddComponent<SFXManager>();

                        DontDestroyOnLoad(singletonGO);
                    }
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }

            Initialize();
        }

        void Initialize()
        {
            GameObject sfxOneShotOrganizerGO = new GameObject("SFXManager_OneShots");
            sfxOneShotOrganizerGO.transform.SetParent(this.transform);
            GameObject sfxLoopOrganizerGO = new GameObject("SFXManager_Loops");
            sfxLoopOrganizerGO.transform.SetParent(this.transform);

            soundPools = new SoundPool(this.transform, sfxOneShotOrganizerGO.transform, sfxLoopOrganizerGO.transform,
                startingOneShotPoolSize, startingLoopPoolSize);
        }

        public void PlayOneShot(SFXOneShot soundEvent, Vector3 soundPosition)
        {
            if (soundEvent.Clip == null)
            {
                Debug.LogWarning("SoundManager.PlayOneShot: No Clip Specified");
                return;
            }

            AudioSource newSource = soundPools.Get();

            newSource.clip = soundEvent.Clip;
            newSource.outputAudioMixerGroup = soundEvent.Mixer;
            newSource.priority = soundEvent.Priority;
            newSource.volume = soundEvent.Volume;
            newSource.pitch = soundEvent.Pitch;
            newSource.panStereo = soundEvent.StereoPan;
            newSource.spatialBlend = soundEvent.SpatialBlend;

            // 3D Options - May not Need
            newSource.minDistance = soundEvent.AttenuationMin;
            newSource.maxDistance = soundEvent.AttenuationMax;

            newSource.transform.position = soundPosition;

            ActivatePooledSound(newSource);
        }

        public AudioSource PlayLoop(SFXLoop soundEvent, Vector3 soundPosition)
        {
            if (soundEvent.Clip == null)
            {
                Debug.LogWarning("SoundManager.PlayLoop: No Clip Specified");
                return null;
            }

            AudioSource newSource = soundPools.GetLoop();

            newSource.clip = soundEvent.Clip;
            newSource.outputAudioMixerGroup = soundEvent.Mixer;
            newSource.priority = soundEvent.Priority;
            newSource.volume = soundEvent.Volume;
            newSource.pitch = soundEvent.Pitch;
            newSource.panStereo = soundEvent.StereoPan;
            newSource.spatialBlend = soundEvent.SpatialBlend;

            // 3D Options - May not Need
            newSource.minDistance = soundEvent.AttenuationMin;
            newSource.maxDistance = soundEvent.AttenuationMax;

            newSource.transform.position = soundPosition;

            if (soundEvent.FiniteLoopingEnabled == false)
            {
                ActivateInfiniteLoopedPooledSound(newSource);
            }
            else
            {
                ActivateFiniteLoopedPooledSound(soundEvent, newSource);
            }

            return newSource;
        }

        private void ActivateInfiniteLoopedPooledSound(AudioSource newSource)
        {
            newSource.gameObject.SetActive(true);
            newSource.Play();
            newSource.loop = true;
        }

        private void ActivateFiniteLoopedPooledSound(SFXLoop soundEvent, AudioSource newSource)
        {
            newSource.gameObject.SetActive(true);

            StartCoroutine(DisableAfterCompleteLoopedRoutine(soundEvent, newSource));
        }

        IEnumerator DisableAfterCompleteLoopedRoutine(SFXLoop soundEvent, AudioSource source)
        {
            int currentRepeat = 0;

            if (!source.isPlaying)
            {
                for (currentRepeat = 0; currentRepeat < soundEvent.NumCycles;)
                {
                    source.Play();

                    float clipDuration = source.clip.length;
                    yield return new WaitForSeconds(clipDuration);

                    source.Stop();

                    currentRepeat++;
                }

                soundPools.Return(source);
            }
        }

        private void ActivatePooledSound(AudioSource newSource)
        {
            newSource.gameObject.SetActive(true);
            newSource.Play();

            StartCoroutine(DisableAfterCompleteRoutine(newSource));
        }

        IEnumerator DisableAfterCompleteRoutine(AudioSource source)
        {
            source.loop = false;

            float clipDuration = source.clip.length;
            yield return new WaitForSeconds(clipDuration);

            source.Stop();

            soundPools.Return(source);
        }
    }
}