using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class CreditsSequence : MonoBehaviour
{
    [SerializeField] private GameObject _creditsContainer;
    [SerializeField] private float _initialPause;
    [SerializeField] private float _scrollDuration;
    [SerializeField] private float _finalYPosition = 6200f;
    
    private void Start()
    {
        StartCoroutine(Scroll());
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            ExitToMainMenu();
    }

    private IEnumerator Scroll()
    {
        yield return new WaitForSecondsRealtime(_initialPause);

        float time = 0;
        var ccPos = _creditsContainer.transform.localPosition;
        var yInitial = ccPos.y;

        while (time < _scrollDuration)
        {
            float y = Mathf.Lerp(yInitial, _finalYPosition, time / _scrollDuration);
            _creditsContainer.transform.localPosition = new Vector3(ccPos.x, y, ccPos.z);
            
            time += Time.deltaTime;
            yield return null;
        }
        
        ccPos = new Vector3(ccPos.x, _finalYPosition, ccPos.z);
        
        ExitToMainMenu();
    }

    private void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
