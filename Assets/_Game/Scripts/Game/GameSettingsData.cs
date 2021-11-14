using UnityEngine;

[CreateAssetMenu]
public class GameSettingsData : ScriptableObject
{
    [Header("Horizontal Movement")]
    [SerializeField] [Range(0, 20)] [Tooltip("The movement speed of the player")]
    private float _moveSpeed = 6.375f;
    [SerializeField] [Range(0, 50)] [Tooltip("The movement acceleration of the player when in the air")]
    private float _airAcceleration = 28f;
    [SerializeField] [Range(0, 1)] [Tooltip("The strength of friction when sliding down a slope")]
    private float _slopeFriction = 0.05f;

    [Header("Vertical Movement")]
    [SerializeField] [Range(0, 10)] [Tooltip("The amount of physics frames (default: 1/50 sec) the game will buffer a jump input")]
    private int _jumpBuffer = 3;
    [SerializeField] [Range(0, 20)] [Tooltip("The jump force of the player")]
    private float _jumpForce = 6.2f;
    [SerializeField] [Range(0, 1)] [Tooltip("How many seconds after leaving solid footing can the player still jump")]
    private float _coyoteJumpTime = 0.3f;
    [SerializeField] [Range(0, 50)] [Tooltip("The force of gravity after jumping")]
    private float _risingGravity = 20f;
    [SerializeField] [Range(0, 50)] [Tooltip("The force of gravity when falling")]
    private float _fallingGravity = 20f;
    [SerializeField] [Range(0, 0.5f)] [Tooltip("How many seconds at the peak of the jump will the player float for")]
    private float _floatTime = 0f;

    [Header("Teleport")]
    [SerializeField] [Range(0, 0.5f)] [Tooltip("The amount of time the player takes to teleport (includes lerping)")]
    private float _teleportTime = 0.1f;
    [SerializeField] [Range(0, 90)] [Tooltip("The amount the FOV should increase while teleporting")]
    private int _teleportFovIncrease = 0;
    [SerializeField] [Range(0, 1)] [Tooltip("The normalized time at which the FOV reaches its max during the teleport, before going back to normal")]
    private float _teleportFovMaxPoint = 1;

    [Header("Camera Settings")]
    [SerializeField] [Range(0, 90)] [Tooltip("0-90 degree lock on the players ability to look down")]
    private float _maxLookDown = 90f;
    [SerializeField] [Range(0, 90)] [Tooltip("0-90 degree lock on the players ability to look up")]
    private float _maxLookUp = 90f;
    [SerializeField] [Tooltip("Whether or not view bobbing is enabled")]
    private bool _viewBobbingEnabled = false;
    [SerializeField] [Range(0, 0.25f)] [Tooltip("The amount view bobbing sways side to side")]
    private float _viewBobbingHorizontal = 0.1f;
    [SerializeField] [Range(0, 0.25f)] [Tooltip("The amount view bobbing sways side to side")]
    private float _viewBobbingVertical = 0.05f;

    [Header("Distance Settings")]
    [SerializeField] [Tooltip("The maximum distance that the UI Indicator can see")]
    private float _maxLookDistance = 20f;
    [SerializeField] [Tooltip("The maximum distance that the Player can interact from")]
    private float _maxInteractDistance = 5;
    [SerializeField] [Tooltip("A layer mask that controls what the UI indicator responds to / can see")]
    private LayerMask _lookAtMask = 1;

    [Header("Action Animation Delays")]
    [SerializeField] [Range(0, 2)] [Tooltip("The delay on when the bolt fires from when the user press the cast input")]
    private float _delayBolt = 0.05f;
    [SerializeField] [Range(0, 2)] [Tooltip("The delay on when the player warps from when the user press the warp input")]
    private float _delayWarp = 0.05f;
    [SerializeField] [Range(0, 2)] [Tooltip("The delay on when the residue activates from when the user press the residue input")]
    private float _delayResidue = 0.05f;

    [Header("Action Animation Time")]
    [SerializeField] [Range(0, 2)] [Tooltip("An extra delay on the bolt casting that happens after it spawns and before it starts moving")]
    private float _timeToFire = 0.05f;

    [Header("Action Cooldowns")]
    [SerializeField] [Range(0, 10)] [Tooltip("A cooldown that limits when the player can cast another bolt")]
    private float _timeToNextBolt = 0.5f;
    [SerializeField] [Range(0, 10)] [Tooltip("A cooldown that limits when the player can warp again")]
    private float _timeToNextWarp = 1f;
    [SerializeField] [Range(0, 10)] [Tooltip("A cooldown that limits when the player can use residue again")]
    private float _timeToNextResidue = 1.5f;
    [SerializeField] [Tooltip("Enable to set additional cooldown on the bolt casting after the player warps")]
    private bool _boltCooldownOnWarp = true;
    [SerializeField] [Range(0, 10)] [Tooltip("An extra cooldown on the bolt ability applied after the player warps")]
    private float _boltTimeAfterWarp = 0.5f;
    [SerializeField] [Range(0, 10)] [Tooltip("An extra cooldown on the warp ability applied after casting in mid-air - to prevent flying")]
    private float _extraWarpTimeInAir = 2f;

    [Header("Action Settings")]
    [SerializeField] [Tooltip("Enable to clear residue from the world when another bolt is casted")]
    private bool _clearResidueOnFire = true;

    [Header("Bolt Movement")]
    [SerializeField] [Range(0, 2)] [Tooltip("The movement speed for the bolt. Multiply by 50 to get total movement in one second")]
    private float _boltMoveSpeed = 0.32f;
    [SerializeField] [Range(0, 2)] [Tooltip("How many seconds the bolt is alive for in total")]
    private float _boltLifeSpan = 1.25f;
    [SerializeField] [Range(0, 1)] [Tooltip("How many seconds of fizzle time the bolt will have (inclusive with Bolt Life Span - Does not add to lifespan)")]
    private float _boltAirFizzleTime = 0.45f;

    [Header("Bolt Visuals")]
    [SerializeField] [Range(0, 1)] [Tooltip("After fizzling, the extra time the bolt is still active. Allows for VFX to disappear naturally")]
    private float _boltAirExtraParticlesTime = 0.6f;
    [SerializeField] [Range(0, 1)] [Tooltip("How many seconds of fizzle time the bolt will have once hitting an object")]
    private float _boltHitFizzleTime = 0.4f;
    [SerializeField] [Range(0, 20)] [Tooltip("How many units downwards the light under the bolt can reach")]
    private float _boltLightDownDist = 8f;
    [SerializeField] [Range(0, 1)] [Tooltip("How many seconds it takes the light to be disabled (fades out)")]
    private float _boltLightDimTime = 0.4f;

    [Header("Bolt Lightning Visuals")]
    [SerializeField] [Tooltip("An animation curve to control the size of the lightning over the bolt's lifespan")]
    private AnimationCurve _lightningSizeOverLife = AnimationCurve.Constant(0, 1, 1);
    [SerializeField] [Tooltip("An animation curve to control the size of the bolt over the bolt's lifespan")]
    private AnimationCurve _boltShellSizeOverLife = AnimationCurve.Constant(0, 1, 1);


    public float MoveSpeed => _moveSpeed;
    public float AirAcceleration => _airAcceleration;
    public float SlopeFriction => _slopeFriction;
    public int JumpBuffer => _jumpBuffer;
    public float JumpForce => _jumpForce;
    public float CoyoteJumpTime => _coyoteJumpTime;
    public float RisingGravity => _risingGravity;
    public float FallingGravity => _fallingGravity;
    public float FloatTime => _floatTime;
    public float TeleportTime => _teleportTime;
    public float TeleportFovIncrease => _teleportFovIncrease;
    public float TeleportFovMaxPoint => _teleportFovMaxPoint;
    public float MaxLookDown => _maxLookDown;
    public float MaxLookUp => _maxLookUp;
    public bool ViewBobbingEnabled => _viewBobbingEnabled;
    public float ViewBobbingHorizontal => _viewBobbingHorizontal;
    public float ViewBobbingVertical => _viewBobbingVertical;
    public float MaxLookDistance => _maxLookDistance;
    public float MaxInteractDistance => _maxInteractDistance;
    public LayerMask LookAtMask => _lookAtMask;
    public float DelayBolt => _delayBolt;
    public float DelayWarp => _delayWarp;
    public float DelayResidue => _delayResidue;
    public float TimeToFire => _timeToFire;
    public float TimeToNextBolt => _timeToNextBolt;
    public float TimeToNextWarp => _timeToNextWarp;
    public float TimeToNextResidue => _timeToNextResidue;
    public bool BoltCooldownOnWarp => _boltCooldownOnWarp;
    public float BoltTimeAfterWarp => _boltTimeAfterWarp;
    public float ExtraWarpTimeInAir => _extraWarpTimeInAir;
    public bool ClearResidueOnFire => _clearResidueOnFire;
    public float BoltMoveSpeed => _boltMoveSpeed;
    public float BoltLifeSpan => _boltLifeSpan - _boltAirFizzleTime;
    public float BoltAirFizzleTime => _boltAirFizzleTime;
    public float BoltAirExtraParticlesTime => _boltAirExtraParticlesTime;
    public float BoltHitFizzleTime => _boltHitFizzleTime;
    public float BoltLightDownDist => _boltLightDownDist;
    public float BoltLightDimTime => _boltLightDimTime;
    public AnimationCurve LightningSizeOverLife => _lightningSizeOverLife;
    public AnimationCurve BoltShellSizeOverLife => _boltShellSizeOverLife;
}