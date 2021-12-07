using System.Collections;
using Mechanics.Dialogue;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;
using AudioSystem;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour, IHoverInteractable
{
    [SerializeField] private TedAnimator _animator = null;

    public string characterName = "";
    public string talkToNode = "";
    private DialogueRunner runner;
    private CustomDialogueUI dialogueUI;
    private int offset, randNum, talk;
    private string[] talks, levelTalks;
    private int talkLimit;
    private int maxTalks = 3;
    private bool levelTalksComplete = false;

    public GameObject interactablePopup, storyPopup, currentPopup;
    private bool hasStory = false, storyFinished = false, firstInteract = true;
    public bool dialogueFinished = true;
    private bool currentSpeaker = false;
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
    private int currentLevel;


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
        currentLevel = SceneManager.GetActiveScene().buildIndex - 1;

        runner = FindObjectOfType<YarnManager>().dialogueRunner;
        dialogueUI = runner.GetComponent<CustomDialogueUI>();

        if (PlayerPrefs.GetString("TedTalks") != "")
            talks = PlayerPrefs.GetString("TedTalks").Split(',');

        if (PlayerPrefs.GetString("LevelTedTalks") != "")
            levelTalks = PlayerPrefs.GetString("LevelTedTalks").Split(',');

        if (characterName == "Ted")
            hasStory = true;

        if (hasStory)
            currentPopup = storyPopup;
        else
            currentPopup = interactablePopup;

        sfxTedAudioSource = voiceOfTed.Play(transform.position);
        if (sfxTedAudioSource) sfxTedAudioSource.Stop();

        currentPopup.SetActive(true);
    }

    void Update()
    {
        if (runner.IsDialogueRunning)
        {

            if (dialogueUI.currentSpeaker == "Ted")
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
        if (dialogueUI.currentSpeaker == "Ted")
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
                firstInteract = false;

            int index = PlayerPrefs.GetInt("Level" + currentLevel + "TedTalkIndex");
            if (index == levelTalks.Length) { levelTalksComplete = true; }

            runner.onDialogueComplete.AddListener(DialogueCompleted);
            dialogueUI.onDialogueExited.AddListener(DialogueExited);
            dialogueUI.onSpeakerChanged?.AddListener(TedSounds);

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

            TedSounds();

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
        if (talkLimit <= maxTalks)
        {
            if (levelTalksComplete)
            {
                if (dialogueFinished)
                    nodeString += GetNextTalk();
                else
                    nodeString += GetCurrentTalk();
            }
            else
            {
                nodeString += currentLevel + "-" + GetNextLevelTalk();
            }
        }
        else
        {
            sfxTedExhaustedCue = false;
            nodeString = "Exhausted";
        }
        return nodeString;
    }

    void DialogueExited()
    {
        if (sfxTedAudioSource) sfxTedAudioSource.Stop();
        UpdateIsTalking(false, false);
        dialogueUI.onSpeakerChanged?.RemoveListener(TedSounds);
        runner.onDialogueComplete.RemoveListener(DialogueCompleted);
        dialogueFinished = false;
        OnBeginHover();
    }

    public void DialogueCompleted()
    {
        if (sfxTedAudioSource) sfxTedAudioSource.Stop();
        dialogueUI.onSpeakerChanged?.RemoveListener(TedSounds);
        dialogueUI.onDialogueEnd?.RemoveListener(DialogueExited);
        runner.onDialogueComplete.RemoveListener(DialogueCompleted);
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
        if (index + 1 >= talks.Length)
        {
            PlayerPrefs.SetInt("TedTalkIndex", 0);
            PlayerPrefs.SetInt("Level1TedTalkIndex", 0);
            PlayerPrefs.SetInt("Level2TedTalkIndex", 0);
            PlayerPrefs.SetInt("Level3TedTalkIndex", 0);
            index = 0;
            AchievementManager.current.unlockAchievement(AchievementManager.Achievements.AllDialogue);
        }
        else
        {
            PlayerPrefs.SetInt("TedTalkIndex", index + 1);
            index += 1;
        }
        return int.Parse(talks[index]);
    }

    public int GetNextLevelTalk()
    {
        int index = PlayerPrefs.GetInt("Level" + currentLevel + "TedTalkIndex");
        if (index == levelTalks.Length)
        {
            levelTalksComplete = true;
        }
        else
        {
            PlayerPrefs.SetInt("Level" + currentLevel + "TedTalkIndex", index + 1);
        }
        return int.Parse(levelTalks[index]);
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