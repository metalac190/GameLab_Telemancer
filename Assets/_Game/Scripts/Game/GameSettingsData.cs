﻿using UnityEngine;

namespace Game
{
    public class GameSettingsData : ScriptableObject
    {
        [Header("Horizontal Movement")]
        [Range(0, 20)] public float moveSpeed = 8.5f;
        [Range(0, 50)] public float airAcceleration = 35f;
        [Header("Vertical Movement")]
        [Range(0, 20)] public float jumpForce = 6.2f;
        [Range(0, 50)] public float risingGravity = 20f;
        [Range(0, 50)] public float fallingGravity = 20f;
        [Range(0, 0.5f)] public float floatTime = 0f;
        [Header("Camera Settings")]
        public float sensitivity = 10f;
        public float maxLookDown = 25f;
        public float maxLookUp = 60f;
        [Header("Distance Settings")]
        public float maxLookDistance = 20f;
        public float maxInteractDistance = 5;
        public float maxBoltLookDistance = 5;
        [Header("Action Animation Delays")]
        [Range(0, 2)] public float delayBolt = 0;
        [Range(0, 2)] public float delayWarp = 0.35f;
        [Range(0, 2)] public float delayResidue = 0.3f;
        [Header("Action Animation Time")]
        [Range(0, 2)] public float timeToFire = 0.35f;
        [Header("Action Cooldowns")]
        [Range(0, 10)] public float timeToNextFire = 0.5f;
        [Range(0, 10)] public float timeToNextWarp = 1f;
        [Range(0, 10)] public float timeToNextResidue = 1.5f;
        [Header("Action Settings")]
        public bool clearResidueOnFire = true;
        [Header("Bolt Movement")]
        [Range(0, 2)] public float movementSpeed = 1;
        public float lifeSpan = 4;
        public float coyoteTime = 0.15f;
    }
}