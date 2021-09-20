using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] Transform _spawnLocation = null;
    [SerializeField] GameObject _projectile = null;

    private GameObject _projectileInstance;

    private void Update()
    {
        Fire();
    }

    public void Fire()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.isPressed) {
            if (_projectileInstance != null) {
                Destroy(_projectileInstance);
            }

            _projectileInstance = Instantiate(_projectile, _spawnLocation.position, Quaternion.identity);
        }
    }
}