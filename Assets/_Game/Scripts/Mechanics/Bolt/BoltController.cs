using System.Collections;
using Mechanics.Player;
using UnityEditor;
using UnityEngine;

namespace Mechanics.Bolt
{
    /// Summary:
    /// The main controller for the bolt warp projectile
    /// Used by Player Casting
    /// Calls OnWarpBoltImpact() on objects that implement IWarpInteractable
    public class BoltController : MonoBehaviour
    {
        [Header("Warp Settings")]
        [SerializeField] private Vector3 _playerRadius = new Vector3(0.45f, 0.9f, 0.45f);
        [SerializeField] private float _collisionCheckDistance = 1;
        [SerializeField] [Range(0, 1)] private float _overCorrection = 0.15f;
        [SerializeField] private LayerMask _collisionMask = 1;
        [SerializeField] private bool _debugWarpBox = false;
        [SerializeField] private bool _forceDontDestroy = false;
        [Header("References")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _visuals;
        [SerializeField] private BoltFeedback _feedback;

        private BoltManager _manager;

        private Vector3 _teleportOffset;
        private Vector3 _prevPosition;

        private bool _checkAlive = true;
        private float _timeAlive;
        private bool _stopMoving;
        private bool _isResidue;

        private Coroutine _redirectDelayRoutine;
        private Coroutine _dissipateRoutine;

        #region Properties

        public bool IsAlive { get; private set; }
        public Collider Collider => _collider;

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
                        Debug.LogError("Missing Bolt Manager in scene", gameObject);
                    }
                }
                return _manager;
            }
            private set => _manager = value;
        }

        #endregion

        #region Unity Functions

        private void OnEnable()
        {
            NullChecks();
        }

        private void Start()
        {
            // No extra bolt controller should exist
            if (_manager == null && !_forceDontDestroy) {
                Debug.LogWarning("No Extra Bolts should exist in scene. Only Bolt Manager");
                Destroy(gameObject);
            }
            IsAlive = gameObject.activeSelf;
        }

        private void Update()
        {
            if (!IsAlive) return;

            CheckLifetime();
        }

        private void FixedUpdate()
        {
            if (_stopMoving) return;

            _prevPosition = transform.position;
            MoveBolt();
            CollisionCheck();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!IsAlive) return;

            var contact = other.GetContact(0);

            Collide(other.gameObject, contact.point, contact.normal);
        }

        private void OnDrawGizmos()
        {
            if (_debugWarpBox) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(transform.position, _playerRadius * 2);
            }
        }

        #endregion

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
            if (!_missingFeedback) {
                _feedback.OnReset();
            }
            SetPosition(position, forward);
            if (_redirectDelayRoutine != null) {
                StopCoroutine(_redirectDelayRoutine);
                _redirectDelayRoutine = null;
            }
            _isResidue = isResidue;
            SetCastStatus(0);
            if (_dissipateRoutine != null) {
                StopCoroutine(_dissipateRoutine);
            }
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

        public bool PrepareToWarp()
        {
            if (!IsAlive || WarpCollisionTesting()) return false;

            if (_dissipateRoutine != null) {
                StopCoroutine(_dissipateRoutine);
            }
            IsAlive = true;
            _stopMoving = true;
            return true;
        }

        // Warp to the bolt's position
        public void OnWarp()
        {
            Manager.BoltData.PlayerController.TeleportToPosition(transform.position, Vector3.down + _teleportOffset);
            Disable();
        }

        #endregion

        #region Private Functions

        private void Collide(GameObject collisionObj, Vector3 collisionPoint, Vector3 collisionNormal)
        {
            IWarpInteractable interactable = collisionObj.GetComponent<IWarpInteractable>();
            if (interactable != null) {
                if (_isResidue) {
                    SetResidue(interactable, collisionPoint, collisionNormal);
                } else {
                    WarpInteract(interactable, collisionPoint, collisionNormal);
                }
            } else {
                Dissipate(true);
                PlayCollisionParticles(collisionPoint, collisionNormal, false);
            }
        }

        private bool WarpCollisionTesting()
        {
            // Ensure that warp bolt position is not out of bounds and space is large enough for player

            bool collision = WarpCollisionCheck();
            if (!collision) {
                _teleportOffset = Vector3.zero;
                return false;
            }

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
                    _teleportOffset = -dist * offset;
                    if (!WarpCollisionCheck()) {
                        //Debug.Log("Warp Collision, adjusting from " + originalPosition + " to " + transform.position);
                        return false;
                    }
                }
            }

            // Could not avoid collision
            _teleportOffset = Vector3.zero;
            Debug.Log("Warp should not be possible (Allowing player to warp anyways)" + transform.position, gameObject);

            // Always let the player teleport -- TODO: Can cause issues, but check ^ doesn't work always right now
            return false;
        }

        // Collision check for the warp bolt. Ignores triggers
        private bool WarpCollisionCheck() => Physics.CheckBox(transform.position + _teleportOffset, _playerRadius, Quaternion.identity, _collisionMask, QueryTriggerInteraction.Ignore);

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
                Dissipate(true);
                PlayCollisionParticles(position, normal, true);
            }
        }

        private void SetResidue(IWarpInteractable interactable, Vector3 position, Vector3 normal)
        {
            Manager.DisableResidue();

            bool activateResidue = interactable.OnSetWarpResidue(Manager.BoltData);
            if (!activateResidue) return;

            Manager.SetResidue(interactable);
            Dissipate(true);
            IsAlive = false;
            PlayCollisionParticles(position, normal, true);
        }

        private void MoveBolt()
        {
            if (_missingRigidbody) return;

            _rb.MovePosition(transform.position + _visuals.forward * PlayerState.Settings.BoltMoveSpeed);
        }

        private void CollisionCheck()
        {
            Vector3 direction = (_prevPosition - _rb.position).normalized * _collisionCheckDistance;
            Ray ray = new Ray(transform.position - direction, direction);
            Physics.Raycast(ray, out var hit, _collisionCheckDistance, _collisionMask, QueryTriggerInteraction.Ignore);
            if (hit.collider == null) return;
            Collide(hit.collider.gameObject, hit.point, hit.normal);
        }

        private void CheckLifetime()
        {
            _timeAlive += Time.deltaTime;
            _feedback.SetBoltLifetime(_timeAlive, PlayerState.Settings.BoltLifeSpan);
            if (!_checkAlive) return;
            if (_timeAlive < PlayerState.Settings.BoltLifeSpan) return;

            float dissipateTime = PlayerState.Settings.BoltAirFizzleTime;
            PrepareToDissipate(dissipateTime);
            float disableTime = PlayerState.Settings.BoltAirExtraParticlesTime;
            _dissipateRoutine = StartCoroutine(LifetimeDissipateTimer(dissipateTime, disableTime));
        }

        private void PrepareToDissipate(float dissipateTime)
        {
            if (!_missingFeedback) {
                float dimLightTime = PlayerState.Settings.BoltLightDimTime;
                _feedback.OnBoltDissipate(transform.position, transform.forward, dissipateTime, dimLightTime);
            }
            if (_dissipateRoutine != null) {
                StopCoroutine(_dissipateRoutine);
            }
            _checkAlive = false;
        }

        public IEnumerator LifetimeDissipateTimer(float dissipateTime, float disableTime)
        {
            for (float t = 0; t < dissipateTime; t += Time.deltaTime) {
                yield return null;
            }
            if (Manager != null) Manager.DissipateBolt(this);
            Disable(false);
            for (float t = 0; t < disableTime; t += Time.deltaTime) {
                yield return null;
            }
            Disable();
        }

        public void Dissipate(bool stopMoving)
        {
            if (!IsAlive) return;
            float dissipateTime = PlayerState.Settings.BoltHitFizzleTime;
            PrepareToDissipate(dissipateTime);
            _checkAlive = false;
            _feedback.OverrideBoltLifetime(_timeAlive, PlayerState.Settings.BoltLifeSpan, PlayerState.Settings.BoltAirFizzleTime, PlayerState.Settings.BoltHitFizzleTime);
            _dissipateRoutine = StartCoroutine(DissipateTimer(dissipateTime, stopMoving));
        }

        private IEnumerator DissipateTimer(float dissipateTime, bool stopMoving)
        {
            Disable(false, stopMoving);
            for (float t = 0; t < dissipateTime; t += Time.deltaTime) {
                yield return null;
            }
            if (Manager != null) Manager.DissipateBolt(this);
            Disable();
        }

        private void PlayCollisionParticles(Vector3 position, Vector3 normal, bool hitInteractable)
        {
            _manager.PlayImpact(position, normal, hitInteractable);
            if (!_missingFeedback) {
                _feedback.OnBoltImpact(position);
            }
        }

        public void Disable(bool returnToController = true, bool stopMoving = true)
        {
            if (!_missingCollider) {
                _collider.enabled = false;
            }
            _stopMoving = stopMoving;
            _checkAlive = false;
            if (returnToController) {
                IsAlive = false;
                if (Manager != null) Manager.AddController(this);
            }
        }

        private void Enable()
        {
            if (!_missingFeedback) {
                _feedback.OnReset();
            }
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
            _stopMoving = false;
            _checkAlive = true;
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region NullCheck

        private void NullChecks()
        {
            VisualsNullCheck();
            RigidbodyNullCheck();
            ColliderNullCheck();
            FeedbackNullCheck();
        }

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