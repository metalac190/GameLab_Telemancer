using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mechanics.Player;
using Yarn.Unity;
using UnityEngine.InputSystem;
using TMPro;

public class YarnManager : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public DialogueRunner tipRunner;
    [SerializeField] private GameObject tipContainer;
    [SerializeField] private PlayerState player;
    [SerializeField] private TextMeshProUGUI dialogueText = null, speaker = null;
    [SerializeField] private CanvasGroup tipGroup = null;
    private int numTalks = 20;
    private int temp;

    void Start()
    {
        // Null checks
        if (dialogueRunner == null || tipRunner == null)
        {
            var runners = FindObjectsOfType<DialogueRunner>();
            foreach(DialogueRunner runner in runners)
            {
                if(runner.name == "DialogueRunner")
                    dialogueRunner = runner;
                else if(runner.name == "TipRunner")
                    tipRunner = runner;
            }
        }

        if (player == null)
            player = FindObjectOfType<PlayerState>();

        // Randomize Ted talks once
        if (PlayerPrefs.GetString("TedTalks") == "")
            RandomizeTedTalks();
    }

    private void Awake()
    {
        UIEvents.current.OnShowTutorials += () => DisplayTutorials(true);
        UIEvents.current.OnHideTutorials += () => DisplayTutorials(false);
    }

    private void DisplayTutorials(bool isEnabled)
    {
        Debug.Log("Changed");
        tipContainer.SetActive(isEnabled);
    }

    public void DialogueStart()
    {
        // Remove all player control when we're in dialogue
        player.OnGamePaused(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void DialogueEnd()
    {
        // Allow player control once dialogue is finished
        player.OnGamePaused(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetText(string textAndSpeaker)
    {
        string[] fullLine = textAndSpeaker.Split(':');
        string text = fullLine[1].Trim(' ');
        string name = fullLine[0];

        dialogueText.text = text;
        speaker.text = name;
    }

    public void RandomizeTedTalks()
    {
        string talkList = "";

        // Initialize and fill array
        int[] talks = new int[numTalks];
        for (int i = 1; i <= numTalks; i++)
        {
            talks[i - 1] = i;
        }

        // Shuffle array
        for (int i = 0; i < numTalks; i++)
        {
            int randNum = Random.Range(0, numTalks);
            temp = talks[randNum];
            talks[randNum] = talks[i];
            talks[i] = temp;
        }

        // Create randomized string and save to PlayerPrefs
        for (int i = 0; i < talks.Length; i++)
        {
            talkList += talks[i];
            if (i < talks.Length - 1)
                talkList += ",";
        }
        PlayerPrefs.SetString("TedTalks", talkList);
        PlayerPrefs.SetInt("TedTalkIndex", 0);
    }

    public void StartTips(float duration){StartCoroutine(FadeInTips(duration));}
    public void StopTips(float duration){StartCoroutine(FadeOutTips(duration));}

    IEnumerator FadeInTips(float duration)
    {
        StopCoroutine(FadeOutTips(duration));
        float time = 0;
        float alphaStart = tipGroup.alpha;
        if(alphaStart > 0)
            duration *= (1 - alphaStart);

        while (time < duration)
        {
            tipGroup.alpha = Mathf.Lerp(alphaStart, 1f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        tipGroup.alpha = 1;
    }

    IEnumerator FadeOutTips(float duration)
    {
        StopCoroutine(FadeInTips(duration));
        float time = 0;
        float alphaStart = tipGroup.alpha;
        if(alphaStart < 1)
            duration *= alphaStart;

        while (time < duration)
        {
            tipGroup.alpha = Mathf.Lerp(alphaStart, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        tipGroup.alpha = 0;
    }
}