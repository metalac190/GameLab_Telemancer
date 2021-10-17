using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TranslateDialogue : MonoBehaviour
{
    bool triggered = false;
    [SerializeField] private GameObject untranslatedTxt, translatedTxt;
    [SerializeField] private Material untranslatedMat, translatedMat;
    [ColorUsageAttribute(true, true)]
    [SerializeField] private Color color;
    private Material utM, tM;
    [SerializeField] private float fadeTime = 1, waitTime = 1;

    private void Start()
    {
        // Set text material to new material instance
        untranslatedTxt.GetComponent<Renderer>().material = untranslatedMat;
        translatedTxt.GetComponent<Renderer>().material = translatedMat;
        utM = untranslatedTxt.GetComponent<Renderer>().material;
        tM = translatedTxt.GetComponent<Renderer>().material;
        
        // Update material color
        utM.SetColor("_Color", color);
        tM.SetColor("_Color", color);

        // Set initial dissolve amounts
        utM.SetFloat("dissolveAmount", 0.001f);
        tM.SetFloat("dissolveAmount", 1f);
    }

    void OnTriggerEnter()
    {
        if (!triggered)
        {
            TriggerDissolve();
        }
    }

    [YarnCommand("StoryDissolve")]
    public void StoryDissolve(){
        TriggerDissolve();
    }

    void TriggerDissolve()
    {
        StartCoroutine(Dissolve(fadeTime));
        StartCoroutine(Undissolve(fadeTime, waitTime));
        triggered = true;
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
