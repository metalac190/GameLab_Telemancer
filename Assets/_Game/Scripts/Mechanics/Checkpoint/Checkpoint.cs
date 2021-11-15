using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Light _light = null;
    [SerializeField] private GameObject _fire = null, _checkpointModel = null;
    [SerializeField] private GameObject _respawnPoint = null;
    [SerializeField] private Material _fireGreen = null, _fireBlue = null, _firePurple = null, _ckptWarm = null, _ckptCool = null;
    private readonly Color _green = new Color(165 / 255f, 255 / 255f, 111 / 255f);
    private readonly Color _blue = new Color(92/255f, 191/255f, 255/255f);
    private readonly Color _purple = new Color(182/255f, 82/255f, 255/255f);
    
    public Transform RespawnPoint => _respawnPoint.transform;

    public event Action<Transform> OnCheckpointReached;

    private void Awake()
    {
        EnableLight(false);

        // Change checkpoint colors based on levels. This is done automatically to avoid interfering with designers.
        var lvl = SceneManager.GetActiveScene().buildIndex - 1;
        Renderer ckptRend = _checkpointModel.GetComponent<Renderer>();
        switch (lvl)
        {
            case 1:
                _light.color = _green;
                SetFireMaterial(_fireGreen);
                ckptRend.material = _ckptWarm;
                break;
            case 2:
                _light.color = _blue;
                SetFireMaterial(_fireBlue);
                ckptRend.material = _ckptCool;
                break;
            default:
                _light.color = _purple;
                SetFireMaterial(_firePurple);
                ckptRend.material = _ckptCool;
                break;
        }
    }

    private void Start()
    {
        UIEvents.current.OnRestartLevel += () => EnableLight(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            OnCheckpointReached?.Invoke(RespawnPoint);
        }
    }

    public void EnableLight()
    {
        _light.enabled = true;
        _fire.SetActive(true);
    }

    public void EnableLight(bool active)
    {
        _light.enabled = active;
        _fire.SetActive(active);
    }

    private void SetFireMaterial(Material coloredFlame)
    {
        Renderer[] flames = _fire.GetComponentsInChildren<Renderer>();
        foreach (var flameRend in flames)
        {
            flameRend.material = coloredFlame;
        }
    }
}
