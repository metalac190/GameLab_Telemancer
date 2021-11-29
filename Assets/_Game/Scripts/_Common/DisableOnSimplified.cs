using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnSimplified : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToDisable;

    private void OnEnable()
    {
        VisualsSelector.OnVisualsChanged += CheckVisuals;
    }

    private void OnDisable()
    {
        VisualsSelector.OnVisualsChanged -= CheckVisuals;
    }

    private void Awake()
    {
        CheckVisuals();
    }

    public void CheckVisuals()
    {
        if (PlayerPrefs.GetFloat("SimplifiedVisuals") == 0)
        {
            SetObjectsEnabled(true);
        }
        else
        {
            SetObjectsEnabled(false);
        }
    }

    public void SetObjectsEnabled(bool enabled)
    {
        foreach(GameObject obj in objectsToDisable)
        {
            obj.SetActive(enabled);
        }
    }
}
