using System.Collections;
using UnityEngine;
using Yarn.Unity;

[RequireComponent(typeof(Collider))]
public class TedTip : MonoBehaviour
{
    public bool showCollidersInGame = false;
    private Collider triggerArea;
    private DialogueRunner runner;
    [SerializeField] string tipNode;
    [SerializeField] private float fadeTime = 1;

    private void Start()
    {
        triggerArea = GetComponent<Collider>();
        GetComponent<MeshRenderer>().enabled = showCollidersInGame;

        if (runner == null)
            runner = FindObjectOfType<YarnManager>().tipRunner;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            EnableTip();
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
            DisableTip();
    }

    void EnableTip()
    {
        Debug.Log("Tip Enabled");
        runner.StartDialogue(tipNode);
    }

    void DisableTip()
    {
        Debug.Log("Tip Disabled");
        runner.onDialogueComplete?.Invoke();
    }

}
