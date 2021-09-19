using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRotator : MonoBehaviour
{
    [SerializeField] float _rotateSpeed = 3f;

    void Update()
    {
        transform.Rotate(0f, 0f, _rotateSpeed * Time.deltaTime, Space.Self);
    }
}
