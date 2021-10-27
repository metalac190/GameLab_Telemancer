using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenu : MonoBehaviour
{
    void Start()
    {
        UIEvents.current.OnRestartLevel += () => gameObject.SetActive(false);
        UIEvents.current.OnPlayerRespawn += () => gameObject.SetActive(false);
    }
}
