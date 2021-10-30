using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionWisp : MonoBehaviour
{
    public Vector3 _Target = new Vector3(0, 0, 0);
    [SerializeField] private float _moveSpeed = 5f;
    private float _tolerance;

    private void Start()
    {
        _tolerance = _moveSpeed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Vector3 _heading = _Target - transform.position;

        if (_heading.magnitude < _tolerance) // if at target, destroy object
        {
            Destroy(gameObject);
        }
        else
        {
            // standard movement
            Vector3 move = (_heading / _heading.magnitude) * _moveSpeed * Time.fixedDeltaTime;
            transform.position += move;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward * 5);
        Gizmos.DrawRay(transform.position, direction);
    }
}
