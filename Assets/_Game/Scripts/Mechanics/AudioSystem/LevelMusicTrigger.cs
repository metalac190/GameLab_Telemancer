using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelMusicTrigger : MonoBehaviour
{
    public UnityEvent OnTriggerMusic;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnTriggerMusic?.Invoke();
            GetComponent<Collider>().enabled = false;
        }
    }
}
