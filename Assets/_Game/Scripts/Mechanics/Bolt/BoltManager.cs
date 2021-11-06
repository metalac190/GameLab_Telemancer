using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mechanics.Bolt
{
    /// Summary:
    /// The pool manager for bolts.
    /// Called by player casting and is the middleman for the Bolt Controllers
    /// Also controls residue
    public class BoltManager : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private BoltController _boltPrefab = null;
        [SerializeField] private int _initialPoolSize = 3;

        private BoltData _boltData;
        private IWarpInteractable _residueInteractable;
        private bool _isCasting;

        // Invoked when the bolt dissipates. True if residue is now ready
        public event Action<bool> OnBoltDissipate = delegate { };

        #region Properties

        public BoltController CurrentBolt { get; private set; }
        public bool CanWarp => CurrentBolt != null && _residueInteractable == null;
        public bool ResidueReady => _residueInteractable != null;
        public bool ReturnAnimationToHold => _residueInteractable.DoesResidueReturnToHoldAnimation();

        public BoltData BoltData
        {
            get
            {
                if (!_boltData.Valid) {
                    _boltData = new BoltData(this, PlayerController);
                    if (!_boltData.Valid) {
                        Debug.Log("Invalid data");
                    }
                }
                return _boltData;
            }
        }

        public PlayerController PlayerController
        {
            get
            {
                if (_playerController == null) {
                    _playerController = FindObjectOfType<PlayerController>();
                }
                return _playerController;
            }
        }

        #endregion

        #region Unity Functions

        private void Awake()
        {
            BuildInitialPool();
        }

        private void OnEnable()
        {
            UIEvents.current.OnPlayerRespawn += OnPlayerRespawn;
        }

        private void OnDisable()
        {
            if (UIEvents.current != null)
                UIEvents.current.OnPlayerRespawn -= OnPlayerRespawn;
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Bolt Pool

        private List<BoltController> _boltControllers = new List<BoltController>();

        private void BuildInitialPool()
        {
            if (_boltPrefab == null) {
                throw new MissingFieldException("Missing Bolt Prefab Reference on " + gameObject);
            }

            for (int i = _boltControllers.Count; i < _initialPoolSize; ++i) {
                CreateNewBolt();
            }
        }

        public void AddController(BoltController controller)
        {
            if (_boltControllers.Contains(controller)) {
                if (CurrentBolt == controller) {
                    CurrentBolt = null;
                }
                return;
            }
            _boltControllers.Add(controller);
            controller.gameObject.SetActive(false);
        }

        private void GetNewBolt()
        {
            if (CurrentBolt != null) {
                CurrentBolt.Dissipate(false);
                CurrentBolt = null;
            }
            if (_boltControllers.Count == 0) {
                CreateNewBolt();
            }
            BoltController controller = _boltControllers[0];
            _boltControllers.Remove(controller);
            CurrentBolt = controller;
            controller.gameObject.SetActive(true);
        }

        private void CreateNewBolt()
        {
            if (_boltPrefab == null) return;
            BoltController newBolt = Instantiate(_boltPrefab, transform);
            newBolt.SetManager(this);
            AddController(newBolt);
        }

        #endregion

        #region Game Wide

        private void OnPlayerRespawn()
        {
            if (CurrentBolt != null) {
                CurrentBolt.Disable();
            }
            CurrentBolt = null;
            _isCasting = false;
            OnBoltDissipate?.Invoke(ResidueReady);
        }

        public void OnGamePaused()
        {
            if (CurrentBolt == null) return;
            CurrentBolt.Disable();
            CurrentBolt = null;
            _isCasting = false;
        }

        #endregion

        // -------------------------------------------------------------------------------------------

        #region Residue

        public bool OnActivateResidue()
        {
            if (!ResidueReady || _residueInteractable == null) return false;
            _residueInteractable.OnActivateWarpResidue(BoltData);
            DisableResidue();
            return true;
        }

        #endregion

        #region Bolt To Manager

        public void SetResidue(IWarpInteractable interactable)
        {
            _residueInteractable = interactable;
        }

        public void DissipateBolt(BoltController controller)
        {
            if (_isCasting || CurrentBolt != controller) return;
            CurrentBolt = null;
            OnBoltDissipate?.Invoke(ResidueReady);
        }

        #endregion

        #region Manager To Bolt

        public void PrepareToFire(Vector3 position, Vector3 forward, bool isResidue)
        {
            GetNewBolt();
            _isCasting = true;
            CurrentBolt.PrepareToFire(position, forward, isResidue);
        }

        public void SetPosition(Vector3 position, Vector3 forward)
        {
            if (!_isCasting) return;
            CurrentBolt.SetPosition(position, forward);
        }

        public void SetCastStatus(float size)
        {
            if (!_isCasting) return;
            CurrentBolt.SetCastStatus(size);
        }

        public void Fire(Vector3 position, Vector3 forward)
        {
            if (!_isCasting) return;
            CurrentBolt.Fire(position, forward);
            _isCasting = false;
        }

        public void RedirectBolt(Vector3 position, Quaternion rotation, float timer)
        {
            if (CurrentBolt != null) {
                CurrentBolt.Disable(true, false);
            }

            GetNewBolt();
            CurrentBolt.Redirect(position, rotation, timer);
        }

        public bool PrepareToWarp()
        {
            if (CurrentBolt == null || _residueInteractable != null) return false;
            return CurrentBolt.PrepareToWarp();
        }

        public void OnWarp()
        {
            if (CurrentBolt == null) return;
            CurrentBolt.OnWarp();
        }

        public void DisableResidue()
        {
            _residueInteractable?.OnDisableWarpResidue();
            _residueInteractable = null;
        }

        #endregion
    }
}