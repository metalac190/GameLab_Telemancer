using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeText : MonoBehaviour
{
    bool triggered = false;
    public ParticleSystem ambientParticles, dissolveParticles;
    [SerializeField] TextMeshPro untranslatedTxt, translatedTxt;
    [SerializeField] private float fadeTime = 1, waitTime = 1;
    private Color trans = new Color(1, 1, 1, 0), solid = new Color(1, 1, 1, 1);

    private void Start()
    {
        translatedTxt.color *= trans;
        untranslatedTxt.color *= solid;
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
            untranslatedTxt.color = Color.Lerp(solid, trans, time / duration);
            Debug.Log(untranslatedTxt.color);
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
            translatedTxt.color = Color.Lerp(trans, solid, time / duration);
            Debug.Log(translatedTxt.color);
            time += Time.deltaTime;
            yield return null;
        }
    }

}