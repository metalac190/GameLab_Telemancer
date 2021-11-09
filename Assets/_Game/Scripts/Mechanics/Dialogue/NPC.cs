using Mechanics.Dialogue;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;

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

    public GameObject interactablePopup, storyPopup, currentPopup;
    private bool hasStory = false, storyFinished = false, firstInteract = true;
    public bool dialogueFinished = true, currentSpeaker = false;

    void Start()
    {
        talkLimit = 0;

        runner = FindObjectOfType<YarnManager>().dialogueRunner;
        dialogueUI = runner.GetComponent<CustomDialogueUI>();

        if (PlayerPrefs.GetString("TedTalks") != "")
            talks = PlayerPrefs.GetString("TedTalks").Split(',');

        if (characterName == "Ted")
            hasStory = true;

        if (hasStory)
            currentPopup = storyPopup;
        else
            currentPopup = interactablePopup;

        currentPopup.SetActive(true);
    }

    void Update()
    {
        if (runner.IsDialogueRunning)
        {
            if (dialogueUI.currentSpeaker == "Ted")
            {
                currentSpeaker = true;
                _animator.SetTalking(true);
            }
            else
            {
                currentSpeaker = false;
                _animator.SetTalking(false);
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                dialogueFinished = false;
                _animator.SetTalking(false);
            }
        }
        else
        {
            currentSpeaker = false;
            _animator.SetTalking(false);
        }
    }

    public void OnInteract()
    {
        if (!runner.IsDialogueRunning)
        {
            if (firstInteract)
            {
                firstInteract = false;
                runner.onDialogueComplete.AddListener(DialogueCompleted);
            }

            currentPopup.SetActive(false);

            // If story beat, run dialogue at specified node
            if (characterName == "Ted" && !storyFinished)
            {
                hasStory = true;
                runner.StartDialogue(talkToNode);
            }
            // Else kick off dialogue at random Ted Talk
            else if (!hasStory || storyFinished)
                runner.StartDialogue(RandomTedTalk());

        }

    }
    public void OnBeginHover()
    {
        currentPopup.SetActive(true);
    }

    public void OnEndHover()
    {
        if (!hasStory)
            currentPopup.SetActive(false);
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
        if (hasStory && !storyFinished)
        {
            storyFinished = true;
            currentPopup.SetActive(false);
            currentPopup = interactablePopup;
        }
        else { talkLimit++; }
        dialogueFinished = true;
        OnBeginHover();
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

    public int GetCurrentTalk()
    {
        int index = PlayerPrefs.GetInt("TedTalkIndex");
        return int.Parse(talks[index]);
    }

    public void SaveTalkNumber(int talkID)
    {
        string newTalks = PlayerPrefs.GetString("TedTalks") + ',' + talkID;
        PlayerPrefs.SetString("TedTalks", newTalks);
    }
}
