using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroText : MonoBehaviour
{
    [SerializeField] private string chapterNumber = "";
    [SerializeField] private string title = "";

    private void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait(){
        yield return new WaitForSeconds(1);
        UIEvents.current?.NotifyChapter(chapterNumber, title);
    }
}
