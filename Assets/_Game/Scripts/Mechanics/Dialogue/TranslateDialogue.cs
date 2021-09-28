using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateDialogue : MonoBehaviour
{
    bool triggered = false;
    public ParticleSystem ambientParticles, dissolveParticles;
    [SerializeField] GameObject untranslated, translated;
    [SerializeField] Material untranslatedMat, translatedMat;
    private Material utM, tM;
    [SerializeField] private float fadeTime = 1, waitTime = 1;

    private void Start()
    {
        untranslated.GetComponent<Renderer>().material = untranslatedMat;
        translated.GetComponent<Renderer>().material = translatedMat;
        utM = untranslated.GetComponent<Renderer>().material;
        tM = translated.GetComponent<Renderer>().material;

        utM.SetFloat("dissolveAmount", 0.001f);
        tM.SetFloat("dissolveAmount", 1f);
    }

    void play()
    {
        ambientParticles.Stop();
        dissolveParticles.Emit(9999);
    }

    void OnTriggerEnter()
    {
        if (!triggered)
        {
            play();
            StartCoroutine(Dissolve(fadeTime));
            StartCoroutine(Undissolve(fadeTime, waitTime));
            triggered = true;
        }
    }

    IEnumerator Dissolve(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            utM.SetFloat("dissolveAmount", Mathf.Lerp(0.001f, 1, time / duration));
            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Undissolve(float duration, float delay)
    {
        float time = 0;
        yield return new WaitForSeconds(delay);
        while (time < duration)
        {
            tM.SetFloat("dissolveAmount", Mathf.Lerp(1f, 0.001f, time / duration));
            time += Time.deltaTime;
            yield return null;
        }
    }

}
