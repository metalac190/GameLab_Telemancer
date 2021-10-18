using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

//[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : LevelActivatable
{
    private enum _movementTypes { LINEAR, LOOP_LINEAR, LOOP_CIRCULAR};
    [Header("MovingPlatform")]
    [SerializeField] private _movementTypes _movementType = _movementTypes.LOOP_LINEAR;
    [SerializeField] private List<Vector3> _path = new List<Vector3>(); // each point along the path the platform will follow. _points[0] should be it's starting position
    private int _currentTarget = 1;
    private int _pathListDirection = 1; // determines wether the platform is moving forwards or backwards through _points

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _delayTime = 2f; // how long the platform should pause at it's destination before moving again
    private float _delayStartTime;
    private float _tolerance;

    [Header("Audio")]
    [SerializeField] private SFXOneShot _movingPlatformSound = null;

    private void Start()
    {
        _tolerance = _moveSpeed * Time.fixedDeltaTime; // the distance moved in one fixed update
    }

    private void FixedUpdate()
    {
        if(IsCurrentlyActive)
        {
            if (transform.position != _path[_currentTarget])
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
        _movingPlatformSound.PlayOneShot(transform.position);
    }

    protected override void OnDeactivate()
    {
        StartCoroutine(MoveToStart());
    }

    protected override void OnReset()
    {
        transform.position = _path[0];
        _currentTarget = 1;
        _pathListDirection = 1;
    }

    // ---------------------------------------------------------------------------------------------------
    #region Movement
    private void MovePlatform()
    {
        // get direction and distance to move
        Vector3 _heading = _path[_currentTarget] - transform.position;

        if (_heading.magnitude < _tolerance) // if distance to move is less than one frame of movement
        {
            transform.position = _path[_currentTarget]; // jump to target
        }
        else
        {
            // standard movement
            transform.position += (_heading / _heading.magnitude) * _moveSpeed * Time.fixedDeltaTime;
            _delayStartTime = Time.time;
        }
    }

    private void UpdateTarget()
    {
        switch(_movementType)
        {
            case _movementTypes.LOOP_LINEAR:
                UpdateTargetLoopLinear();
                break;
            case _movementTypes.LOOP_CIRCULAR:
                UpdateTargetLoopCircular();
                break;
            case _movementTypes.LINEAR:
                UpdateTargetLinear();
                break;
        }
    }

    // after platform reaches it's target, get the next target, and wait if at either end of path
    private void UpdateTargetLoopLinear()
    {
        if (Time.time - _delayStartTime >= _delayTime) // wait for _delayTime to move targets
        {
            // get next target
            _currentTarget += _pathListDirection;
            if (_currentTarget < 0 || _currentTarget >= _path.Count)
            {
                _pathListDirection = _pathListDirection * -1;
                _currentTarget += _pathListDirection * 2;
                Mathf.Clamp(_currentTarget, 0, _path.Count);
            }
        }
        else if (_currentTarget > 0 && _currentTarget < _path.Count - 1) // if in middle of path, don't wait for _delayTime
        {
            _currentTarget += _pathListDirection;
        }
    }

    private void UpdateTargetLoopCircular()
    {
        _currentTarget += _pathListDirection;
        if (_currentTarget >= _path.Count)
            _currentTarget = 0;
    }

    private void UpdateTargetLinear()
    {
        if (_currentTarget >= _path.Count - 1)
            return;
        else
            _currentTarget += _pathListDirection;
    }

    IEnumerator MoveToStart()
    {
        if (_currentTarget != 0)
        {
            _currentTarget--;
            _pathListDirection = -1;
        }
        while (transform.position != _path[0])
        {
            if (transform.position != _path[_currentTarget])
            {
                MovePlatform();
            }
            else
            {
                UpdateTargetLoopLinear();
            }
            yield return new WaitForFixedUpdate();
        }
        _currentTarget = 0;
        _pathListDirection = 1;
    }

    #endregion
    // ---------------------------------------------------------------------------------------------------

    //other.gameObject.layer != LayerMask.NameToLayer("Player Trigger"))

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Warp Bolt") &&
                other.gameObject.layer != LayerMask.NameToLayer("Ground Detector") &&
                other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            //if (other.transform.root != other.transform)
            //other.transform.parent = transform;
            other.transform.root.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Warp Bolt") &&
                other.gameObject.layer != LayerMask.NameToLayer("Ground Detector") &&
                other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            Transform exitingObj = other.gameObject.transform;
            while(exitingObj.parent != transform)
            {
                exitingObj = exitingObj.parent;
            }
            exitingObj.parent = null;
        }
    }

    

    private void OnDrawGizmos()
    {
        if(_path.Count > 0)
        {
            Gizmos.DrawWireSphere(_path[0], 0.25f);
            if(_movementType != _movementTypes.LOOP_CIRCULAR)
                Gizmos.DrawWireSphere(_path[_path.Count - 1], 0.25f);

            for (int i = 0; i < _path.Count - 1; i++)
            {
                Gizmos.DrawLine(_path[i], _path[i + 1]);
            }

            if (_movementType == _movementTypes.LOOP_CIRCULAR)
                Gizmos.DrawLine(_path[0], _path[_path.Count - 1]);
        }
    }
}
