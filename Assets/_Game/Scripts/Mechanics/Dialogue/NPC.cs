using UnityEngine;
using Yarn.Unity;

public class NPC : MonoBehaviour, IHoverInteractable
{

    public string characterName = "";
    public string talkToNode = "";
    private DialogueRunner runner;

    [Header("Optional")]
    public YarnProgram scriptToLoad;
    public GameObject interactablePopup;

    void Start()
    {
        if (runner == null)
            runner = FindObjectOfType<Yarn.Unity.DialogueRunner>();

        if (scriptToLoad != null)
            runner.Add(scriptToLoad);
    }

    public void OnInteract()
    {
        // If story beat, run dialogue at specified node
        if (characterName == "Ted")
            runner.StartDialogue(talkToNode);
        // Else kick off dialogue at random Ted Talk
        else
            runner.StartDialogue(RandomTedTalk());

        // TODO: Find way to hide popup during conversation and reenable after
        // OnEndHover();
    }
    public void OnBeginHover()
    {
        Debug.Log("Begin Hover");
        interactablePopup.SetActive(true);
    }

    public void OnEndHover()
    {
        Debug.Log("End Hover");
        interactablePopup.SetActive(false);
    }

    public string RandomTedTalk()
    {
        string nodeString = "TedTalk";
        nodeString += Random.Range(1, 20);
        return nodeString;
    }
}
