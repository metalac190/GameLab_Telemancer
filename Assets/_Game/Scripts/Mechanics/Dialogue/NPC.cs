using UnityEngine;
using Yarn.Unity;
using System.Linq;
using System.Collections.Generic;

public class NPC : MonoBehaviour, IHoverInteractable
{

    public string characterName = "";
    public string talkToNode = "";
    private DialogueRunner runner;
    private int offset, randNum, talk;
    private string[] talks;
    private int talkLimit;

    [Header("Optional")]
    public YarnProgram scriptToLoad;
    public GameObject interactablePopup;

    void Start()
    {
        talkLimit = 0;

        if (runner == null)
            runner = FindObjectOfType<Yarn.Unity.DialogueRunner>();

        if (scriptToLoad != null)
            runner.Add(scriptToLoad);

        if (PlayerPrefs.GetString("TedTalks") != "")
            talks = PlayerPrefs.GetString("TedTalks").Split(',');
    }

    public void OnInteract()
    {
        if (!runner.IsDialogueRunning)
        {
            interactablePopup.SetActive(false);
            // If story beat, run dialogue at specified node
            if (characterName == "Ted")
                runner.StartDialogue(talkToNode);
            // Else kick off dialogue at random Ted Talk
            else
                runner.StartDialogue(RandomTedTalk());
        }

        // TODO: Find way to hide popup during conversation and reenable after
        // OnEndHover();
    }
    public void OnBeginHover()
    {
        // Debug.Log("Begin Hover");
        interactablePopup.SetActive(true);
    }

    public void OnEndHover()
    {
        // Debug.Log("End Hover");
        interactablePopup.SetActive(false);
    }

    public string RandomTedTalk()
    {
        string nodeString = "TedTalk";
        if (talkLimit < 5)
        {
            nodeString += GetNextTalk();
            talkLimit++;
        }
        else
            nodeString = "Exhausted";
        return nodeString;
    }

    public int GetNextTalk()
    {
        int index = PlayerPrefs.GetInt("TedTalkIndex");
        if (index + 1 == talks.Length)
            PlayerPrefs.SetInt("TedTalkIndex", 0);
        else
            PlayerPrefs.SetInt("TedTalkIndex", index + 1);
        return int.Parse(talks[index]);
    }

    public void SaveTalkNumber(int talkID)
    {
        string newTalks = PlayerPrefs.GetString("TedTalks") + ',' + talkID;
        PlayerPrefs.SetString("TedTalks", newTalks);
    }
}
