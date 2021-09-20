using UnityEngine;

// The main controller for the bolt warp projectile
// Used by Player Casting
// Calls OnWarpBoltImpact() on objects that implement IWarpInteractable
namespace Mechanics.WarpBolt
{
    public class BoltController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] [Range(0, 2)] private float _movementSpeed = 1;
        [SerializeField] private float _lifeSpan = 4;
        [Header("References")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _visuals;
        [SerializeField] private BoltFeedback _feedback;
        [SerializeField] private BoltData _data;
        public BoltData BoltData => GetBoltData();

        private bool _isAlive;
        private float _timeAlive;

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
                _collider.isTrigger = true;
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

        #region Unity Functions

        private void OnEnable()
        {
            VisualsNullCheck();
            RigidbodyNullCheck();
            ColliderNullCheck();

            Disable();
            BoltData.ResetDirection(Vector3.zero);
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

        // On Collision Enter causes some weird issues on impact
        // Warning: If you want to switch to collision, change the value in Null Check to not make it trigger!
        private void OnTriggerEnter(Collider other)
        {
            if (!_isAlive) return;

            IWarpInteractable interactable = other.GetComponent<IWarpInteractable>();
            if (interactable != null) {
                WarpInteract(interactable);
            }
            Dissipate();
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Public Functions

        // Player is starting to cast the bolt
        public void PrepareToFire(Vector3 position, Quaternion rotation)
        {
            if (_isAlive) {
                Dissipate();
            }
            if (!_missingVisuals) {
                _visuals.gameObject.SetActive(true);
                SetPosition(position, rotation);
            }
            SetCastStatus(0);
        }

        // Update the bolt's position. Called to keep the bolt in the player's hand
        public void SetPosition(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            _visuals.rotation = rotation;
            if (!_missingVisuals) {
            }
        }

        // Size should go from 0 to 1 as the player is casting the bolt
        public void SetCastStatus(float size)
        {
            _visuals.localScale = new Vector3(size, size, size);
        }

        // Set the bolts position and direction and fire the bolt
        public void Fire(Vector3 position, Vector3 direction)
        {
            BoltData.ResetDirection(direction);
            transform.position = position;
            if (!_missingCollider) {
                _collider.enabled = true;
            }
            _isAlive = true;
            _timeAlive = 0;
        }

        // Warp to the bolt's position
        public void OnWarp()
        {
            if (!_missingFeedback) {
                _feedback.OnPlayerWarp();
            }
            _data.PlayerController.Teleport(transform);
            Disable();
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Private Functions

        private void WarpInteract(IWarpInteractable interactable)
        {
            bool dissipate = interactable.OnWarpBoltImpact(BoltData);
            if (!_missingFeedback) {
                _feedback.OnWarpInteract();
            }

            if (dissipate) Dissipate();
        }

        private void MoveBolt()
        {
            if (_missingRigidbody) return;
            _rb.MovePosition(transform.position + BoltData.Direction * _movementSpeed);
        }

        private void CheckLifetime()
        {
            _timeAlive += Time.deltaTime;
            if (_timeAlive > _lifeSpan) {
                Dissipate();
            }
        }

        private void Dissipate()
        {
            if (!_missingFeedback) {
                _feedback.OnBoltDissipate();
            }
            Disable();
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

        #endregion
    }
}