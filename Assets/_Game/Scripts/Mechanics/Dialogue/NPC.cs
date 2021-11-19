using System.Collections;
using Mechanics.Dialogue;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;
using AudioSystem;

public class NPC : MonoBehaviour, IHoverInteractable
{
    [SerializeField] private TedAnimator _animator = null;

    public string characterName = "";
    public string talkToNode = "";
    private DialogueRunner runner;
    private CustomDialogueUI dialogueUI;
    private int offset, randNum, talk;
    private string[] talks;
    private int talkLimit;
    private int maxTalks = 3;

    public GameObject interactablePopup, storyPopup, currentPopup;
    private bool hasStory = false, storyFinished = false, firstInteract = true;
    public bool dialogueFinished = true;
    private bool currentSpeaker = false;
    private bool test = false;
    public bool CurrentSpeaker
    {
        get { return currentSpeaker; }
        set
        {
            if (value == currentSpeaker) return;

            currentSpeaker = value;
            UpdateIsTalking(inConversation, currentSpeaker);
        }
    }

    [Header("SFX Hookup")]
    [SerializeField] SFXLoop voiceOfTed = null;
    private AudioSource sfxTedAudioSource;
    private bool sfxTedExhaustedCue = false;

    private bool isTalking;
    private static bool inConversation;
    private static bool allowGamePausing;

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

        if (voiceOfTed)
        {
            sfxTedAudioSource = voiceOfTed?.Play(transform.position);
            sfxTedAudioSource.Stop();
            sfxTedAudioSource.loop = true;
        }
        dialogueUI.onSpeakerChanged?.AddListener(TedSounds);
    }

    void Update()
    {
        if (runner.IsDialogueRunning)
        {

            if (dialogueUI.currentSpeaker == characterName)
            {
                currentSpeaker = true;
                UpdateIsTalking(true, true);
            }
            else
            {
                currentSpeaker = false;
                UpdateIsTalking(true, false);
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                dialogueFinished = false;
                UpdateIsTalking(false, false);
            }
        }
        else
        {
            if (talkLimit >= maxTalks && sfxTedExhaustedCue == false)
            {
                if (sfxTedAudioSource) sfxTedAudioSource.Stop();
                sfxTedExhaustedCue = true;
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (sfxTedAudioSource) sfxTedAudioSource.Stop();
            }

            currentSpeaker = false;
            UpdateIsTalking(false, false);
        }
        if (allowGamePausing)
        {
            if (Keyboard.current.escapeKey.wasReleasedThisFrame || !Keyboard.current.escapeKey.isPressed)
            {
                UIEvents.current.EnableGamePausing();
                allowGamePausing = false;
                //Debug.Log("Allow Pausing");
            }
        }
    }

    private void UpdateIsTalking(bool conversation, bool talking)
    {
        if (conversation != inConversation)
        {
            //Debug.Log("Start Conversation");
            inConversation = conversation;
            if (conversation)
            {
                UIEvents.current.DisableGamePausing();
            }
            else
            {
                allowGamePausing = true;
                //Debug.Log("End Conversation");
            }
        }
        if (talking == isTalking) return;
        isTalking = talking;
        _animator.SetTalking(talking);
    }

    private void TedSounds()
    {
        if (dialogueUI.currentSpeaker == characterName)
        {
            sfxTedAudioSource.Play();
        }
        else { if (sfxTedAudioSource) sfxTedAudioSource.Stop(); }
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
        if (talkLimit < maxTalks)
        {
            nodeString += GetNextTalk();
        }
        else
        {
            sfxTedExhaustedCue = false;
            nodeString = "Exhausted";
            runner.onDialogueComplete.RemoveListener(DialogueCompleted);
            dialogueUI.onSpeakerChanged.RemoveListener(TedSounds);
        }
        return nodeString;
    }

    public void DialogueCompleted()
    {
        sfxTedAudioSource.Stop();
        UpdateIsTalking(false, false);
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