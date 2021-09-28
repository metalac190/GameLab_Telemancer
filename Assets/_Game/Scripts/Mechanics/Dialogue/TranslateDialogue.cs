using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TranslateDialogue : MonoBehaviour
{
    bool triggered = false;
    public ParticleSystem ambient, particles;
    [SerializeField] GameObject untranslated, translated;
    [SerializeField] Material untranslatedMat, translatedMat;
    private Material utM, tM;
    [SerializeField] private float fadeTime = 1, waitTime = 1;

    private void Start()
    {
        untranslated.GetComponent<Renderer>().material = new Material(untranslatedMat);
        translated.GetComponent<Renderer>().material = new Material(translatedMat);
        utM = untranslated.GetComponent<Renderer>().material;
        tM = translated.GetComponent<Renderer>().material;

        utM.SetFloat("dissolveAmount", 0.001f);
        tM.SetFloat("dissolveAmount", 1f);
    }

    void play()
    {
        ambient.Stop();
        particles.Emit(9999);

    }

    IEnumerator waitForIt()
    {
        yield return new WaitForSeconds(1.5f);
        play();
    }


    void OnTriggerEnter()
    {
        if (!triggered)
        {
            play();
            StartCoroutine(FadeColorOut(fadeTime));
            StartCoroutine(FadeColorIn(fadeTime, waitTime));
            triggered = true;
        }
    }

    IEnumerator FadeColorOut(float duration)
    {
        float time = 0;

        while (time < duration)
        {
            utM.SetFloat("dissolveAmount", Mathf.Lerp(0.001f, 1, time / duration));
            // translated.color = Color.Lerp(trans, solid, time / duration);
            // Debug.Log(untranslated.color);
            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeColorIn(float duration, float delay)
    {
        float time = 0;
        yield return new WaitForSeconds(delay);
        while (time < duration)
        {
            tM.SetFloat("dissolveAmount", Mathf.Lerp(1f, 0.001f, time / duration));
            // Debug.Log(untranslated.color);
            time += Time.deltaTime;
            yield return null;
        }
    }

}
