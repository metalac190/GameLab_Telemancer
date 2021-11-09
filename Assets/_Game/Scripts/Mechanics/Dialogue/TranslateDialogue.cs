using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class TranslateDialogue : MonoBehaviour, IPlayerInteractable
{
    bool triggered = false;
    [SerializeField] private GameObject untranslatedTxt = null, translatedTxt = null;
    [SerializeField] private Material untranslatedMat = null, translatedMat = null;
    [ColorUsageAttribute(true, true)]
    [SerializeField] private Color color = Color.white;
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

    public void OnInteract()
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
        GetComponent<Collider>().enabled = false;
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
