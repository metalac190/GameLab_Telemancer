using Mechanics.Dialogue;
using UnityEngine;
using Yarn.Unity;

public class NPC : MonoBehaviour, IHoverInteractable
{
    [SerializeField] private TedAnimator _animator;

    public string characterName = "";
    public string talkToNode = "";
    private DialogueRunner runner;
    private CustomDialogueUI dialogueUI;
    private int offset, randNum, talk;
    private string[] talks;
    private int talkLimit;

    [Header("Optional")]
    public GameObject interactablePopup;
    bool hasStory = false, storyFinished = false;
    public bool currentSpeaker = false;

    void Start()
    {
        talkLimit = 0;

        runner = FindObjectOfType<YarnManager>().dialogueRunner;
        dialogueUI = runner.GetComponent<CustomDialogueUI>();

        if (PlayerPrefs.GetString("TedTalks") != "")
            talks = PlayerPrefs.GetString("TedTalks").Split(',');
    }

    void Update()
    {
        if (runner.IsDialogueRunning && dialogueUI.currentSpeaker == "Ted") {
            _animator.SetTalking(true);
            currentSpeaker = true;
        } else {
            _animator.SetTalking(false);
            currentSpeaker = false;
        }
    }

    public void OnInteract()
    {
        if (!runner.IsDialogueRunning)
        {
            interactablePopup.SetActive(false);
            runner.onDialogueComplete.AddListener(DialogueCompleted);

            // If story beat, run dialogue at specified node
            if (characterName == "Ted")
            {
                hasStory = true;
                runner.StartDialogue(talkToNode);
            }
            // Else kick off dialogue at random Ted Talk
            else
                runner.StartDialogue(RandomTedTalk());
        }

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
        }
        else
        {
            nodeString = "Exhausted";
            runner.onDialogueComplete.RemoveListener(DialogueCompleted);
        }
        return nodeString;
    }

    public void DialogueCompleted()
    {
        _animator.SetTalking(false);
        OnBeginHover();
        talkLimit++;
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
