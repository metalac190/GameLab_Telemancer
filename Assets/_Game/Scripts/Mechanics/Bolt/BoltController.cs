﻿using System;
using System.Collections;
using UnityEngine;

namespace Mechanics.Bolt
{
    /// Summary:
    /// The main controller for the bolt warp projectile
    /// Used by Player Casting
    /// Calls OnWarpBoltImpact() on objects that implement IWarpInteractable
    public class BoltController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] [Range(0, 2)] private float _movementSpeed = 1;
        [SerializeField] private float _lifeSpan = 4;
        [SerializeField] private float _coyoteTime = 0.15f;
        [Header("Warping")]
        [SerializeField] private Vector3 _playerRadius = new Vector3(0.45f, 0.9f, 0.45f);
        [SerializeField] [Range(0, 1)] private float _overCorrection = 0.15f;
        [SerializeField] private LayerMask _collisionMask = 1;
        [SerializeField] private bool _debugWarpBox = false;
        [Header("References")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _visuals;
        [SerializeField] private BoltFeedback _feedback;

        private bool _isResidue;
        private float _timeAlive;

        private Coroutine _redirectDelayRoutine;

        private BoltManager _manager;

        public BoltManager Manager
        {
            get
            {
                if (_manager == null) {
                    Transform parent = transform.parent;
                    if (parent != null) {
                        _manager = GetComponent<BoltManager>();
                    }
                    if (_manager == null) {
                        throw new MissingReferenceException("Missing Bolt Manager in scene");
                    }
                }
                return _manager;
            }
            private set => _manager = value;
        }

        public bool IsAlive { get; private set; }

        // -------------------------------------------------------------------------------------------

        #region Unity Functions

        private void OnEnable()
        {
            VisualsNullCheck();
            RigidbodyNullCheck();
            ColliderNullCheck();
            FeedbackNullCheck();
        }

        private void Start()
        {
            // No extra bolt controller should exist
            if (_manager == null) {
                Debug.Log("No Extra Bolts should exist in scene. Only Bolt Manager");
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (!IsAlive) return;

            CheckLifetime();
        }

        private void FixedUpdate()
        {
            if (!IsAlive) return;

            MoveBolt();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!IsAlive) return;

            var contact = other.GetContact(0);

            IWarpInteractable interactable = other.gameObject.GetComponent<IWarpInteractable>();
            if (interactable != null) {
                if (_isResidue) {
                    SetResidue(interactable, contact.point, contact.normal);
                } else {
                    WarpInteract(interactable, contact.point, contact.normal);
                }
            } else {
                Dissipate(true, true);
                PlayCollisionParticles(contact.point, contact.normal, false);
            }
        }

        #endregion

        private void OnDrawGizmos()
        {
            if (_debugWarpBox) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(transform.position, _playerRadius * 2);
            }
        }

        // -------------------------------------------------------------------------------------------

        #region Public Functions

        public void SetManager(BoltManager manager)
        {
            Manager = manager;
        }

        public void Redirect(Vector3 position, Quaternion rotation, float timer)
        {
            if (timer == 0 || !_isResidue) {
                FinishRedirect(position, rotation);
            } else {
                _redirectDelayRoutine = StartCoroutine(RedirectDelay(position, rotation, timer));
            }
        }

        // Called when the player presses the "cast bolt" button
        public void PrepareToFire(Vector3 position, Vector3 forward, bool isResidue)
        {
            _visuals.gameObject.SetActive(true);
            SetPosition(position, forward);
            if (_redirectDelayRoutine != null) {
                StopCoroutine(_redirectDelayRoutine);
                _redirectDelayRoutine = null;
            }
            _isResidue = isResidue;
            SetCastStatus(0);
        }

        // Update the bolt's position. Called to keep the bolt in the player's hand
        public void SetPosition(Vector3 position, Vector3 forward)
        {
            transform.position = position;
            _visuals.forward = forward;
        }

        // Size should go from 0 to 1 as the player is casting the bolt
        public void SetCastStatus(float delta)
        {
            _feedback.SetBoltCastDelta(delta);
        }

        // Set the bolts position and direction and fire the bolt
        public void Fire(Vector3 position, Vector3 forward)
        {
            transform.position = position;
            _visuals.forward = forward;
            Enable();
        }

        // Warp to the bolt's position
        public bool OnWarp()
        {
            return Warp();
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Private Functions

        private bool Warp()
        {
            if (WarpCollisionTesting()) return false;

            // TODO: Fix this line
            Manager.BoltData.PlayerController.TeleportToPosition(transform.position, Vector3.down);
            Disable();
            return true;
        }

        private bool WarpCollisionTesting()
        {
            // Ensure that warp bolt position is not out of bounds and space is large enough for player

            bool collision = WarpCollisionCheck();
            if (!collision) return false;

            // Offsets to try
            Vector3 originalPosition = transform.position;
            Vector3[] checkOffsets =
            {
                new Vector3(0, -_playerRadius.y, 0),
                new Vector3(0, _playerRadius.y, 0),
                new Vector3(_playerRadius.x, 0, 0),
                new Vector3(-_playerRadius.x, 0, 0),
                new Vector3(0, 0, _playerRadius.z),
                new Vector3(0, 0, -_playerRadius.z)
            };


            // Attempt to avoid collision at each of the offsets
            foreach (var offset in checkOffsets) {
                bool hitObj = Physics.Linecast(originalPosition, originalPosition + offset, out var hit);
                if (hitObj) {
                    float dist = (offset.magnitude - hit.distance) / offset.magnitude + _overCorrection;
                    transform.position = originalPosition - dist * offset;
                    if (!WarpCollisionCheck()) {
                        Debug.Log("Warp Collision, adjusting from " + originalPosition + " to " + transform.position);
                        return false;
                    }
                }
            }

            // Could not avoid collision
            transform.position = originalPosition;
            Debug.Log("Warp Failed: Not enough space in area");
            return true;
        }

        // Collision check for the warp bolt. Ignores triggers
        private bool WarpCollisionCheck() => Physics.CheckBox(transform.position, _playerRadius, Quaternion.identity, _collisionMask, QueryTriggerInteraction.Ignore);

        private IEnumerator RedirectDelay(Vector3 position, Quaternion rotation, float timer)
        {
            Disable();
            yield return new WaitForSecondsRealtime(timer);
            FinishRedirect(position, rotation);
        }

        private void FinishRedirect(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            _visuals.forward = rotation * Vector3.forward;
            Enable();
        }

        private void WarpInteract(IWarpInteractable interactable, Vector3 position, Vector3 normal)
        {
            bool dissipate = interactable.OnWarpBoltImpact(Manager.BoltData);
            if (!_missingFeedback) {
                _feedback.OnWarpInteract();
            }

            if (dissipate) {
                Dissipate(true, false);
                PlayCollisionParticles(position, normal, true);
            }
        }

        private void SetResidue(IWarpInteractable interactable, Vector3 position, Vector3 normal)
        {
            Manager.DisableResidue();

            bool activateResidue = interactable.OnSetWarpResidue(Manager.BoltData);
            if (activateResidue) {
                Manager.SetResidue(interactable);
                Dissipate(true, false);
                PlayCollisionParticles(position, normal, true);
            }
        }

        private void MoveBolt()
        {
            if (_missingRigidbody) return;

            _rb.MovePosition(transform.position + _visuals.forward * _movementSpeed);
        }

        private void CheckLifetime()
        {
            if (!IsAlive) return;
            _timeAlive += Time.deltaTime;
            if (_timeAlive > _lifeSpan) {
                Dissipate(false, true);
            }
        }

        public void Dissipate(bool stopMoving, bool coyoteTime)
        {
            if (!IsAlive) return;
            float dissipateTime = 0;
            if (!_missingFeedback) {
                dissipateTime = _feedback.OnBoltDissipate(transform.position, transform.forward);
            }
            StartCoroutine(DissipateTimer(dissipateTime, stopMoving, coyoteTime));
        }

        private IEnumerator DissipateTimer(float dissipateTime, bool stopMoving, bool coyoteTime)
        {
            if (!_missingCollider) {
                _collider.enabled = false;
            }
            if (stopMoving) IsAlive = false;
            _timeAlive = 0;
            if (coyoteTime) {
                yield return new WaitForSecondsRealtime(_coyoteTime);
            }
            Manager.DissipateBolt();
            float timer = Mathf.Max(0, coyoteTime ? dissipateTime - _coyoteTime : dissipateTime);
            yield return new WaitForSecondsRealtime(timer);
            Disable();
        }

        private void PlayCollisionParticles(Vector3 position, Vector3 normal, bool hitInteractable)
        {
            if (!_missingFeedback) {
                _feedback.OnBoltImpact(position, normal, hitInteractable);
            }
        }

        public void Disable()
        {
            if (!_missingCollider) {
                _collider.enabled = false;
            }
            IsAlive = false;
            Manager.AddController(this);
        }

        private void Enable()
        {
            if (!_missingCollider) {
                _collider.enabled = true;
            }
            if (!_missingRigidbody) {
                // Ensure that the rigidbody doesn't have any velocity
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
            IsAlive = true;
            _timeAlive = 0;
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region NullCheck

        private void VisualsNullCheck()
        {
            if (_visuals == null) {
                _visuals = transform.Find("Visuals");
                if (_visuals == null) {
                    _visuals = transform.Find("Art");
                    if (_visuals == null) {
                        _visuals = transform;
                        Debug.LogWarning("Cannot find Warp Bolt Visuals", gameObject);
                    }
                }
            }
        }

        private bool _missingRigidbody;

        private void RigidbodyNullCheck()
        {
            if (_rb == null) {
                if (transform.parent != null) {
                    _rb = transform.parent.GetComponent<Rigidbody>();
                }
                if (_rb == null) {
                    _rb = GetComponent<Rigidbody>();
                    if (_rb == null) {
                        _missingRigidbody = true;
                        Debug.LogWarning("Cannot find Warp Bolt Rigidbody", gameObject);
                    }
                }
            }
        }

        private bool _missingCollider;

        private void ColliderNullCheck()
        {
            if (_collider == null) {
                if (transform.parent != null) {
                    _collider = transform.parent.GetComponent<Collider>();
                }
                if (_collider == null) {
                    _collider = GetComponent<Collider>();
                    if (_collider == null) {
                        _missingCollider = true;
                        Debug.LogWarning("Cannot find Warp Bolt Collider", gameObject);
                    }
                }
            }
            if (_collider != null) {
                _collider.isTrigger = false;
            }
        }

        private bool _missingFeedback;

        private void FeedbackNullCheck()
        {
            if (_feedback == null) {
                _feedback = transform.GetComponentInChildren<BoltFeedback>();
                if (_feedback == null) {
                    _missingFeedback = true;
                    Debug.LogWarning("Cannot find Warp Bolt Collider", gameObject);
                }
            }
        }

        #endregion
    }
}