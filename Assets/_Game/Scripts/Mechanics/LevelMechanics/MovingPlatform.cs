using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : LevelActivatable
{
    [Header("MovingPlatform")]
    [SerializeField] private List<Vector3> _points; // each point along the path the platform will follow. _points[0] should be it's starting position
    private int _currentTarget = 1;
    private int _pointsListDirection = 1; // determines wether the platform is moving forwards or backwards through _points

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _delayTime = 2f; // how long the platform should pause at it's destination before moving again
    private float _delayStartTime;
    private float _tolerance; 

    private void Start()
    {
        _tolerance = _moveSpeed * Time.fixedDeltaTime; // the distance moved in one fixed update
    }

    private void FixedUpdate()
    {
        if(IsCurrentlyActive)
        {
            if (transform.position != _points[_currentTarget])
            {
                MovePlatform();
            }
            else
            {
                UpdateTarget();
            }
        }
    }

    protected override void OnActivate()
    {
        // vfx & sfx?
    }

    protected override void OnDeactivate()
    {
        StartCoroutine(MoveToStart());
    }

    private void MovePlatform()
    {
        // get direction and distance to move
        Vector3 _heading = _points[_currentTarget] - transform.position;

        if (_heading.magnitude < _tolerance) // if distance to move is less than one frame of movement
        {
            transform.position = _points[_currentTarget]; // jump to target
        }
        else
        {
            // standard movement
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
            if (_currentTarget < 0 || _currentTarget >= _points.Count)
            {
                _pointsListDirection = _pointsListDirection * -1;
                _currentTarget += _pointsListDirection * 2;
                Mathf.Clamp(_currentTarget, 0, _points.Count);
            }
        }
        else if (_currentTarget > 0 && _currentTarget < _points.Count - 1) // if in middle of path, don't wait for _delayTime
        {
            _currentTarget += _pointsListDirection;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.parent = transform;
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;
    }

    IEnumerator MoveToStart()
    {
        while(transform.position != _points[0])
        {
            _currentTarget = 0;
            _pointsListDirection = 1;
            MovePlatform();
            yield return new WaitForFixedUpdate();
        }
    }
}
