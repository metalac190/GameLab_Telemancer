using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRotator : MonoBehaviour
{
    [SerializeField] float _rotateSpeed = 3f;

    [SerializeField] GameObject _lightningTarget;
    [SerializeField] float _LTRotSpeed = -3f;

    void Update()
    {
        transform.Rotate(0f, 0f, _rotateSpeed * Time.deltaTime, Space.Self);
        //_lightningTarget.transform.Rotate(_LTRotSpeed * 0.25f * Time.deltaTime, _LTRotSpeed * Time.deltaTime, 0f, Space.Self);
    }
}
