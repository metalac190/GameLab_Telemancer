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
    [SerializeField] [Range(0, 2)] private float _delayBolt = 0.35f;
    [SerializeField] [Range(0, 2)] private float _delayWarp = 0.1f;
    [SerializeField] [Range(0, 2)] private float _delayResidue = 0.1f;

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


    public float MoveSpeed => _moveSpeed;
    public float AirAcceleration => _airAcceleration;
    public float JumpForce => _jumpForce;
    public float RisingGravity => _risingGravity;
    public float FallingGravity => _fallingGravity;
    public float FloatTime => _floatTime;
    public float MaxLookDown => _maxLookDown;
    public float MaxLookUp => _maxLookUp;
    public float MaxLookDistance => _maxLookDistance;
    public float MaxInteractDistance => _maxInteractDistance;
    public LayerMask LookAtMask => _lookAtMask;
    public float DelayBolt => _delayBolt;
    public float DelayWarp => _delayWarp;
    public float DelayResidue => _delayResidue;
    public float TimeToFire => _timeToFire;
    public float TimeToNextFire => _timeToNextFire;
    public float TimeToNextWarp => _timeToNextWarp;
    public float TimeToNextResidue => _timeToNextResidue;
    public bool ClearResidueOnFire => _clearResidueOnFire;
    public float BoltMoveSpeed => _boltMoveSpeed;
    public float BoltLifeSpan => _boltLifeSpan;
    public float BoltAirFizzleTime => _boltLightDownDist;
    public float BoltLightDownDist => _boltAirFizzleTime;
    public float BoltHitFizzleTime => _boltHitFizzleTime;
    public float GrowDuration => _growDuration;
    public float GrowDrag => _growDrag;
    public float ShrinkDuration => _shrinkDuration;
    public float ShrinkDrag => _shrinkDrag;
}