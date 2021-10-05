using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalker : MonoBehaviour
{
    [Header("EnemyWalker")]
    [SerializeField] private List<Vector3> _path = new List<Vector3>(); // each point along the path the platform will follow. _points[0] should be it's starting position
    private int _currentTarget = 1;
    private int _pointsListDirection = 1; // determines wether the platform is moving forwards or backwards through _points

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _delayTime = 0f; // how long the platform should pause at it's destination before moving again
    private float _delayStartTime;
    private float _tolerance;

    private void Start()
    {
        _tolerance = _moveSpeed * Time.fixedDeltaTime; // the distance moved in one fixed update
    }

    private void FixedUpdate()
    {
        if (transform.position != _path[_currentTarget])
        {
            Move();
        }
        else
        {
            UpdateTarget();
        }
    }

    private void Move()
    {
        // get direction and distance to move
        Vector3 _heading = _path[_currentTarget] - transform.position;

        if (_heading.magnitude < _tolerance) // if distance to move is less than one frame of movement
        {
            //_rb.MovePosition(_path[_currentTarget]); // jump to target
            transform.position = _path[_currentTarget]; // jump to target
        }
        else
        {
            // standard movement
            //_rb.MovePosition(transform.position + ((_heading / _heading.magnitude) * _moveSpeed * Time.fixedDeltaTime));
            transform.position += (_heading / _heading.magnitude) * _moveSpeed * Time.fixedDeltaTime;
            _delayStartTime = Time.time;
        }
    }

    // after platform reaches it's target, get the next target, and wait if at either end of path
    private void UpdateTarget()
    {
        if (Time.time - _delayStartTime >= _delayTime) // wait for _delayTime to move targets
        {
            // get next target
            _currentTarget += _pointsListDirection;
            if (_currentTarget < 0 || _currentTarget >= _path.Count)
            {
                _pointsListDirection = _pointsListDirection * -1;
                _currentTarget += _pointsListDirection * 2;
                Mathf.Clamp(_currentTarget, 0, _path.Count);
            }
        }
        else if (_currentTarget > 0 && _currentTarget < _path.Count - 1) // if in middle of path, don't wait for _delayTime
        {
            _currentTarget += _pointsListDirection;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Kill player");
            other.GetComponent<Mechanics.Player.PlayerState>()?.OnKill();
        }   
    }

    private void OnDrawGizmos()
    {
        if (_path.Count > 0)
        {
            Gizmos.DrawWireSphere(_path[0], 0.25f);
            Gizmos.DrawWireSphere(_path[_path.Count - 1], 0.25f);
            for (int i = 0; i < _path.Count - 1; i++)
            {
                Gizmos.DrawLine(_path[i], _path[i + 1]);
            }
        }
    }
}
