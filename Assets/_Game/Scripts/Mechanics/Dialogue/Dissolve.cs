using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dissolve : MonoBehaviour
{
    bool triggered = false;
    public ParticleSystem ambient;
    public ParticleSystem particles;
    [SerializeField] TextMeshPro untranslated;
    [SerializeField] TextMeshPro translated;
    Color untranslatedColor, translatedColor;
    [SerializeField] private float fadeTime = 1;
    [SerializeField] private float waitTime = 1;
    private Color trans = new Color(1, 1, 1, 0);
    private Color solid = new Color(1, 1, 1, 1);

    private void Start() {
        translated.color *= trans;
        untranslated.color *= solid;
    }

    void play(){
        ambient.Stop();
        particles.Emit(9999);

    }

    IEnumerator waitForIt(){
        yield return new WaitForSeconds(1.5f);
        play();
    }
    
    
    void OnTriggerEnter(){
        if(!triggered){
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
            untranslated.color = Color.Lerp(solid, trans, time / duration);
            Debug.Log(untranslated.color);
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
            translated.color = Color.Lerp(trans, solid, time / duration);
            Debug.Log(untranslated.color);
            time += Time.deltaTime;
            yield return null;
        }
    }

}