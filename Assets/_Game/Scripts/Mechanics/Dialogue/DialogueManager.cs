using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mechanics.Player;
using Yarn.Unity;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueRunner runner;
    [SerializeField] private PlayerState player;
    [SerializeField] private TextMeshProUGUI dialogueText, speaker;
    private int numTalks = 20;
    private int temp;

    void Start()
    {
        // Null checks
        if (runner == null)
            runner = FindObjectOfType<DialogueRunner>();
        if (player == null)
            player = FindObjectOfType<PlayerState>();

        // Randomize Ted talks once
        if (PlayerPrefs.GetString("TedTalks") != "")
            RandomizeTedTalks();
    }

    public void DialogueStart()
    {
        // Remove all player control when we're in dialogue
        player.GamePaused(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void DialogueEnd()
    {
        // Allow player control once dialogue is finished
        player.GamePaused(false);
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
}