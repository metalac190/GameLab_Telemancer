using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayStoneWisps : MonoBehaviour
{
    [SerializeField] private GameObject _relayParent = null;
    private GameObject _relayParentTarget = null;
    private GameObject _relayPairTarget = null;
    private Vector3 Target;
    private bool _approachingParent = false;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _tolerance = 1f;

    //slerp
    private Vector3 _slerpStartPos;
    [SerializeField] private float _slerpTime = 1f;
    [SerializeField] private float _slerpSpeed = 1f;
    private float _startTime;
    private Vector3 _centerPoint;
    private Vector3 _startRelCenter;
    private Vector3 _endRelCenter;
    private Vector3 _slerpDirection;

    private enum MovementState {STANDARD, SLERP_IN, SLERP_OUT };
    private MovementState _moveState = MovementState.STANDARD;

    private void Start()
    {
        _relayParentTarget = _relayParent.GetComponent<RelayStone>().BoltSource;
        _relayPairTarget = _relayParent.GetComponent<RelayStone>().RelayPair.BoltSource;
        Target = _relayPairTarget.transform.position;
        _slerpDirection = Vector3.right;
    }

    private void FixedUpdate()
    {
        switch(_moveState)
        {
            case MovementState.STANDARD:
                StandardMovement();
                break;

            case MovementState.SLERP_IN:
                SlerpMovement();
                break;
            case MovementState.SLERP_OUT:
                SlerpMovement();
                break;
        }
    }

    private void StandardMovement()
    {
        Vector3 _heading = Target - transform.position;

        if (_heading.magnitude < _tolerance) // if at target, destroy object
        {
            _startTime = Time.time;
            _slerpStartPos = transform.position;
            _moveState = MovementState.SLERP_IN;
            Debug.Log("Slerping In");

            transform.parent = _approachingParent ? _relayParent.transform : _relayParent.GetComponent<RelayStone>().RelayPair.gameObject.transform;
        }
        else
        {
            // standard movement
            Vector3 move = (_heading / _heading.magnitude) * _moveSpeed * Time.fixedDeltaTime;
            transform.position += move;
        }
    }

    private void SlerpMovement()
    {
        GetCenter(_slerpDirection);
        //float fracComplete = Mathf.PingPong(Time.time - _startTime, _slerpTime / _moveSpeed);
        float fracComplete = (Time.time - _startTime) * _slerpTime / _slerpSpeed;
        Vector3 move = Vector3.Slerp(_startRelCenter, _endRelCenter, fracComplete * _slerpSpeed);
        move += _centerPoint;

        Vector3 heading = move - transform.position;
        transform.rotation = Quaternion.LookRotation(heading);
        transform.position = move;

        if(fracComplete >= 1)
        {
            if (_moveState == MovementState.SLERP_IN)
            {
                Debug.Log("Slerping out");
                Vector3 temp = _slerpStartPos;
                _slerpStartPos = Target;
                Target = temp;
                _startTime = Time.time;
                _slerpDirection = _slerpDirection * (-1);
                _moveState = MovementState.SLERP_OUT;
            }
            else
            {
                _slerpDirection = _slerpDirection * (-1);
                Debug.Log("return to standard");
                _approachingParent = !_approachingParent;
                Target = _approachingParent ? _relayParentTarget.transform.position : _relayPairTarget.transform.position;
                _moveState = MovementState.STANDARD;

                transform.parent = null;
            }
        }
    }

    public void GetCenter(Vector3 direction)
    {
        _centerPoint = (_slerpStartPos + Target) * 0.5f;
        _centerPoint -= direction;
        _startRelCenter = _slerpStartPos - _centerPoint;
        _endRelCenter = Target - _centerPoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward * 5);
        Gizmos.DrawRay(transform.position, direction);
    }
}
