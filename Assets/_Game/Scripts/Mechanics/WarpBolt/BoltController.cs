using System;
using System.Collections;
using UnityEngine;

namespace Mechanics.WarpBolt
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
        [SerializeField] private BoltData _data;
        public BoltData BoltData => GetBoltData();
        public bool ResidueReady { get; private set; }
        public event Action OnResidueReady = delegate { };
        public event Action OnWarpDissipate = delegate { };

        private IWarpInteractable _residueInteractable = null;

        private bool _isResidue;
        private bool _isAlive;
        private float _timeAlive;

        private Vector3 _previousPosition;
        private Coroutine _redirectDelayRoutine = null;

        // -------------------------------------------------------------------------------------------

        #region Unity Functions

        private void OnEnable()
        {
            VisualsNullCheck();
            RigidbodyNullCheck();
            ColliderNullCheck();
            FeedbackNullCheck();

            Disable();
            BoltData.Direction = Vector3.zero;
        }

        private void Update()
        {
            if (!_isAlive) return;

            CheckLifetime();
        }

        private void FixedUpdate()
        {
            if (!_isAlive) return;

            MoveBolt();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!_isAlive) return;

            var contact = other.GetContact(0);

            IWarpInteractable interactable = other.gameObject.GetComponent<IWarpInteractable>();
            if (interactable != null) {
                if (_isResidue) {
                    SetResidue(interactable, contact.point, contact.normal);
                } else {
                    WarpInteract(interactable, contact.point, contact.normal);
                }
            } else {
                Dissipate();
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

        public void Redirect(Transform reference, float timer)
        {
            Redirect(reference.position, reference.rotation, timer);
        }

        public void Redirect(Vector3 position, Quaternion rotation, float timer)
        {
            if (timer == 0) {
                transform.position = position;
                _visuals.forward = rotation * Vector3.forward;
                _data.Direction = rotation * Vector3.forward;
            } else {
                _redirectDelayRoutine = StartCoroutine(RedirectDelay(position, rotation, timer));
            }
        }

        // Called when the player presses the "cast bolt" button
        public void PrepareToFire(Vector3 position, Vector3 forward, bool isResidue)
        {
            if (_isAlive) {
                Dissipate();
            }
            if (!_missingVisuals) {
                _visuals.gameObject.SetActive(true);
                SetPosition(position, forward);
            }
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
            if (!_missingVisuals) {
                _visuals.forward = forward;
            }
        }

        // Size should go from 0 to 1 as the player is casting the bolt
        public void SetCastStatus(float size)
        {
            _visuals.localScale = new Vector3(size, size, size);
        }

        // Set the bolts position and direction and fire the bolt
        public void Fire(Vector3 position, Vector3 forward)
        {
            BoltData.Direction = forward;
            transform.position = position;
            if (!_missingVisuals) {
                _visuals.forward = forward;
            }
            if (!_missingCollider) {
                _collider.enabled = true;
            }
            if (!_missingRigidbody) {
                // Ensure that the rigidbody doesn't have any velocity
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
            _isAlive = true;
            _timeAlive = 0;
        }

        public bool CanWarp()
        {
            return _isAlive;
        }

        // Warp to the bolt's position
        public bool OnWarp()
        {
            return _isAlive && Warp();
        }

        public bool CanUseResidue()
        {
            return ResidueReady && _residueInteractable != null;
        }

        public bool OnActivateResidue()
        {
            if (!ResidueReady || _residueInteractable == null) return false;
            _residueInteractable.OnActivateWarpResidue(BoltData);
            DisableResidue();
            return true;
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Private Functions

        private bool Warp()
        {
            if (WarpCollisionTesting()) return false;
            if (!_missingFeedback) {
                _feedback.OnPlayerWarp();
            }
            _data.PlayerController.TeleportToPosition(transform.position, Vector3.down);
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
            Enable();
            transform.position = position;
            _visuals.forward = rotation * Vector3.forward;
            _data.Direction = rotation * Vector3.forward;
        }

        private void WarpInteract(IWarpInteractable interactable, Vector3 position, Vector3 normal)
        {
            bool dissipate = interactable.OnWarpBoltImpact(BoltData);
            if (!_missingFeedback) {
                _feedback.OnWarpInteract();
            }

            if (dissipate) {
                Dissipate();
                PlayCollisionParticles(position, normal, true);
            }
        }

        private void SetResidue(IWarpInteractable interactable, Vector3 position, Vector3 normal)
        {
            DisableResidue();

            bool activateResidue = interactable.OnSetWarpResidue(BoltData);
            if (activateResidue) {
                ResidueReady = true;
                OnResidueReady?.Invoke();
                _residueInteractable = interactable;
                Dissipate();
                PlayCollisionParticles(position, normal, true);
            }
        }

        private void MoveBolt()
        {
            if (_missingRigidbody) return;
            _previousPosition = transform.position;

            _rb.MovePosition(transform.position + BoltData.Direction * _movementSpeed);
        }

        private void CheckLifetime()
        {
            _timeAlive += Time.deltaTime;
            if (_timeAlive > _lifeSpan) {
                Dissipate();
            }
        }

        public void DisableResidue()
        {
            _residueInteractable?.OnDisableWarpResidue();
            _residueInteractable = null;
            ResidueReady = false;
        }

        public void Dissipate()
        {
            if (!_missingFeedback) {
                _feedback.OnBoltDissipate(transform.position, transform.forward);
            }
            OnWarpDissipate?.Invoke();
            Disable();
        }

        private void PlayCollisionParticles(Vector3 position, Vector3 normal, bool hitInteractable)
        {
            if (!_missingFeedback) {
                _feedback.OnBoltImpact(position, normal, hitInteractable);
            }
        }

        private void Disable()
        {
            if (!_missingVisuals) {
                _visuals.gameObject.SetActive(false);
            }
            if (!_missingCollider) {
                _collider.enabled = false;
            }
            _isAlive = false;
        }

        private void Enable()
        {
            if (!_missingVisuals) {
                _visuals.gameObject.SetActive(true);
            }
            if (!_missingCollider) {
                _collider.enabled = true;
            }
            _isAlive = true;
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region NullCheck

        private BoltData GetBoltData()
        {
            if (_data == null) {
                _data = (BoltData)ScriptableObject.CreateInstance("BoltData");
                _data.SetWarpBoltReference(this);
            }
            return _data;
        }

        private bool _missingVisuals;

        private void VisualsNullCheck()
        {
            if (_visuals == null) {
                _visuals = transform.Find("Art");
                if (_visuals == null) {
                    _missingVisuals = true;
                    Debug.LogWarning("Cannot find Warp Bolt Visuals", gameObject);
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