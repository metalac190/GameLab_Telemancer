using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNotificationText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Color _chNumColor = Color.white;
    [SerializeField] private float _chNumFadeIn = 0.8f;
    [SerializeField] private float _Pause = 1f;
    [SerializeField] private float _FadeOut = 1.2f;
    
    private void Start()
    {
        // on ui event
        UIEvents.current.OnNotifyPlayer += s =>
        {
            StopAllCoroutines();
            StartCoroutine(FadeText(s));
        };
        _text.gameObject.SetActive(false);
    }

    private IEnumerator FadeText(string str)
    {
        _text.text = str;

        float time = 0;
        float alphaL1, alphaL2;
        float targetA1 = _chNumColor.a;

        // set both lines invisible
        _text.gameObject.SetActive(true);
        _text.color = new Color(_chNumColor.r, _chNumColor.g, _chNumColor.b, 0);

        // wait a little bit
        //yield return new WaitForSecondsRealtime(1f);

        // fade in line one
        while (time < _chNumFadeIn) {
            alphaL1 = Mathf.Lerp(0, targetA1, time / _chNumFadeIn);
            _text.color = new Color(_chNumColor.r, _chNumColor.g, _chNumColor.b, alphaL1);
            time += Time.deltaTime;
            yield return null;
        }
        _text.color = new Color(_chNumColor.r, _chNumColor.g, _chNumColor.b, targetA1);

        // wait a little bit
        yield return new WaitForSecondsRealtime(_Pause);

        // fade out both
        time = 0;
        while (time < _FadeOut) {
            alphaL1 = Mathf.Lerp(1, 0, time / _FadeOut);
            _text.color = new Color(_chNumColor.r, _chNumColor.g, _chNumColor.b, alphaL1);
            time += Time.deltaTime;
            yield return null;
        }
        _text.color = new Color(_chNumColor.r, _chNumColor.g, _chNumColor.b, 0);

        // disable both
        _text.gameObject.SetActive(false);
    }
}
