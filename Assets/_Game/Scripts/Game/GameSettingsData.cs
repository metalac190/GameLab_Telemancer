using UnityEngine;

[CreateAssetMenu]
public class GameSettingsData : ScriptableObject
{
    [Header("Horizontal Movement")]
    [SerializeField] [Range(0, 20)] private float _moveSpeed = 8.5f;
    [SerializeField] [Range(0, 50)] private float _airAcceleration = 35f;

    [Header("Vertical Movement")]
    [SerializeField] [Range(0, 20)] private float _jumpForce = 6.2f;
    [SerializeField] [Range(0, 50)] private float _risingGravity = 20f;
    [SerializeField] [Range(0, 50)] private float _fallingGravity = 20f;
    [SerializeField] [Range(0, 0.5f)] private float _floatTime = 0f;

    [Header("Camera Settings")]
    [SerializeField] private float _maxLookDown = 90f;
    [SerializeField] private float _maxLookUp = 90f;

    [Header("Distance Settings")]
    [SerializeField] private float _maxLookDistance = 20f;
    [SerializeField] private float _maxInteractDistance = 5;
    [SerializeField] private LayerMask _lookAtMask = 1;

    [Header("Action Animation Delays")]
    [SerializeField] [Range(0, 2)] private float _delayBolt = 0;
    [SerializeField] [Range(0, 2)] private float _delayWarp = 0.35f;
    [SerializeField] [Range(0, 2)] private float _delayResidue = 0.3f;

    [Header("Action Animation Time")]
    [SerializeField] [Range(0, 2)] private float _timeToFire = 0.35f;

    [Header("Action Cooldowns")]
    [SerializeField] [Range(0, 10)] private float _timeToNextFire = 0.5f;
    [SerializeField] [Range(0, 10)] private float _timeToNextWarp = 1f;
    [SerializeField] [Range(0, 10)] private float _timeToNextResidue = 1.5f;

    [Header("Action Settings")]
    [SerializeField] private bool _clearResidueOnFire = true;

    [Header("Bolt Movement")]
    [SerializeField] [Range(0, 2)] private float _boltMoveSpeed = 0.32f;
    [SerializeField] private float _boltLifeSpan = 0.7f;
    [SerializeField] private float _boltAirFizzleTime = 0.6f;

    [Header("Bolt Visuals")]
    [SerializeField] private float _boltLightDownDist = 8f;
    [SerializeField] private float _boltHitFizzleTime = 0.5f;
    [SerializeField] private float _growDuration = 3f;
    [SerializeField] private float _growDrag = 8f;
    [SerializeField] private float _shrinkDuration = 1.2f;
    [SerializeField] private float _shrinkDrag = 4f;


    public float moveSpeed => _moveSpeed;
    public float airAcceleration => _airAcceleration;
    public float jumpForce => _jumpForce;
    public float risingGravity => _risingGravity;
    public float fallingGravity => _fallingGravity;
    public float floatTime => _floatTime;
    public float maxLookDown => _maxLookDown;
    public float maxLookUp => _maxLookUp;
    public float maxLookDistance => _maxLookDistance;
    public float maxInteractDistance => _maxInteractDistance;
    public LayerMask lookAtMask => _lookAtMask;
    public float delayBolt => _delayBolt;
    public float delayWarp => _delayWarp;
    public float delayResidue => _delayResidue;
    public float timeToFire => _timeToFire;
    public float timeToNextFire => _timeToNextFire;
    public float timeToNextWarp => _timeToNextWarp;
    public float timeToNextResidue => _timeToNextResidue;
    public bool clearResidueOnFire => _clearResidueOnFire;
    public float boltMoveSpeed => _boltMoveSpeed;
    public float boltLifeSpan => _boltLifeSpan;
    public float boltAirFizzleTime => _boltLightDownDist;
    public float boltLightDownDist => _boltAirFizzleTime;
    public float boltHitFizzleTime => _boltHitFizzleTime;
    public float growDuration => _growDuration;
    public float growDrag => _growDrag;
    public float shrinkDuration => _shrinkDuration;
    public float shrinkDrag => _shrinkDrag;
}