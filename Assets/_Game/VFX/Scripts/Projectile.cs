using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _speed = 3f;

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * _speed * Time.fixedDeltaTime);
    }
}
