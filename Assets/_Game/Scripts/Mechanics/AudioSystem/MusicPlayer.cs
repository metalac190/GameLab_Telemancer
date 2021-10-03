using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public class MusicPlayer : MonoBehaviour
    {
        List<AudioSource> layerSources = new List<AudioSource>();
        List<float> sourceStartVolumes = new List<float>();
        MusicEvent _musicEvent = null;

        Coroutine fadeVolumeRoutine = null;
        Coroutine stopRoutine = null;

        private void Awake()
        {
            CreateLayerSources();
        }

        void CreateLayerSources()
        {
            for (int i = 0; i < MusicManager.MaxLayerCount; i++)
            {
                layerSources.Add(gameObject.AddComponent<AudioSource>());

                layerSources[i].playOnAwake = false;
                layerSources[i].loop = true;
            }
        }

        public void Play(MusicEvent musicEvent, float fadeTime)
        {
            if (musicEvent == null)
            {
                Debug.LogWarning("MusicEvent is empty, cannot play.");
                return;
            }

            _musicEvent = musicEvent;

            for (int i = 0; i < layerSources.Count
                && (i < musicEvent.MusicLayers.Length); i++)
            {
                if (musicEvent.MusicLayers[i] != null)
                {
                    // If content is in music layer, assign it
                    layerSources[i].volume = 0;
                    layerSources[i].clip = musicEvent.MusicLayers[i];
                    layerSources[i].outputAudioMixerGroup = musicEvent.Mixer;
                    layerSources[i].Play();
                }
            }

            FadeVolume(MusicManager.Instance.Volume, fadeTime);
        }

        public void Stop(float fadeTime)
        {
            if (stopRoutine != null)
                StopCoroutine(stopRoutine);
            stopRoutine = StartCoroutine(StopRoutine(fadeTime));
        }

        IEnumerator StopRoutine(float fadeTime)
        {
            // Cancels current running volume fades
            if (fadeVolumeRoutine != null)
                StopCoroutine(fadeVolumeRoutine);

            // Blends volume to 0 depending on layer type
            if (_musicEvent.LayerType == LayerType.Additive)
            {
                fadeVolumeRoutine = StartCoroutine(LerpSourceAdditiveRoutine(0, fadeTime));
            }
            else if (_musicEvent.LayerType == LayerType.Single)
            {
                fadeVolumeRoutine = StartCoroutine(LerpSourceSingleRoutine(0, fadeTime));
            }

            // Waits for blend to finish and then stops audio sources
            yield return fadeVolumeRoutine;

            foreach (AudioSource source in layerSources)
            {
                source.Stop();
            }
        }

        public void FadeVolume(float targetVolume, float fadeTime)
        {
            if (_musicEvent == null) return;

            // Animates each audiosource.volume over time
            targetVolume = Mathf.Clamp(targetVolume, 0, 1);
            if (fadeTime < 0) fadeTime = 0;

            if (fadeVolumeRoutine != null)
                StopCoroutine(fadeVolumeRoutine);

            if (_musicEvent.LayerType == LayerType.Additive)
            {
                fadeVolumeRoutine = StartCoroutine
                    (LerpSourceAdditiveRoutine(targetVolume, fadeTime));
            }
            else if (_musicEvent.LayerType == LayerType.Single)
            {
                fadeVolumeRoutine =
                    StartCoroutine(LerpSourceSingleRoutine(targetVolume, fadeTime));
            }
        }

        IEnumerator LerpSourceAdditiveRoutine(float targetVolume, float fadeTime)
        {
            SaveSourceStartVolumes();

            float startVolume;
            float newVolume;

            for (float elapsedTime = 0; elapsedTime <= fadeTime; elapsedTime += Time.deltaTime)
            {
                for (int i = 0; i < layerSources.Count; i++)
                {
                    // If active layer, fade to target
                    if (i <= MusicManager.Instance.ActiveLayerIndex)
                    {
                        startVolume = sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeTime);
                        layerSources[i].volume = newVolume;
                    }
                    // Fades to 0 from current volume if not
                    else
                    {
                        startVolume = sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeTime);
                        layerSources[i].volume = newVolume;
                    }
                }

                yield return null;
            }

            // If we get this far, set to target for accuracy - whole # instead of decimals
            for (int i = 0; i < layerSources.Count; i++)
            {
                if (i <= MusicManager.Instance.ActiveLayerIndex)
                {
                    layerSources[i].volume = targetVolume;
                }
                else
                {
                    layerSources[i].volume = 0;
                }
            }
        }

        private void SaveSourceStartVolumes()
        {
            sourceStartVolumes.Clear();
            for (int i = 0; i < layerSources.Count; i++)
            {
                sourceStartVolumes.Add(layerSources[i].volume);
            }
        }

        IEnumerator LerpSourceSingleRoutine(float targetVolume, float fadeTime)
        {
            SaveSourceStartVolumes();

            float startVolume;
            float newVolume;

            for (float elapsedTime = 0; elapsedTime <= fadeTime; elapsedTime += Time.deltaTime)
            {
                for (int i = 0; i < layerSources.Count; i++)
                {
                    // If active layer, blend up
                    if (i == MusicManager.Instance.ActiveLayerIndex)
                    {
                        startVolume = sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeTime);
                        layerSources[i].volume = newVolume;
                    }
                    // Otherwise blend down inactive layers
                    else
                    {
                        startVolume = sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeTime);
                        layerSources[i].volume = newVolume;
                    }
                }

                yield return null;
            }

            // If we get this far, set to target for accuracy - whole # instead of decimals
            for (int i = 0; i < layerSources.Count; i++)
            {
                if (i == MusicManager.Instance.ActiveLayerIndex)
                {
                    layerSources[i].volume = targetVolume;
                }
                else
                {
                    layerSources[i].volume = 0;
                }
            }
        }
    }
}
