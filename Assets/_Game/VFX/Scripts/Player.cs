using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform _spawnLocation;
    [SerializeField] GameObject _projectile;

    private GameObject _projectileInstance;

    private void Update()
    {
        Fire();
    }

    public void Fire()
    {
        if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0))
        {
            if(_projectileInstance != null)
            {
                Destroy(_projectileInstance);
            }

            _projectileInstance = Instantiate(_projectile, _spawnLocation.position, Quaternion.identity);
        }
    }
}
