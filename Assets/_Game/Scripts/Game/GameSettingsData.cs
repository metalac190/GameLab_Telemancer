using UnityEngine;

namespace Game
{
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
        [SerializeField] private float _sensitivity = 10f;
        [SerializeField] private float _maxLookDown = 25f;
        [SerializeField] private float _maxLookUp = 60f;
        [Header("Distance Settings")]
        [SerializeField] private float _maxLookDistance = 20f;
        [SerializeField] private float _maxInteractDistance = 5;
        [SerializeField] private float _maxBoltLookDistance = 5;
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
        [SerializeField] [Range(0, 2)] private float _movementSpeed = 1;
        [SerializeField] private float _lifeSpan = 4;
        [SerializeField] private float _coyoteTime = 0.15f;
    }
}