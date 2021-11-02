using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.Player;
using UnityEditor;
using UnityEngine;

namespace Mechanics.Bolt
{
    public class BoltManager : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private BoltController _boltPrefab = null;
        [SerializeField] private int _initialPoolSize = 3;

        private BoltData _boltData;

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

        private bool _isCasting = false;
        private IWarpInteractable _residueInteractable;

        public BoltController CurrentBolt => _currentBolt;
        public bool CanWarp => _currentBolt != null && _residueInteractable == null;
        public bool ResidueReady => _residueInteractable != null;
        public event Action OnResidueReady = delegate { };
        public event Action<bool> OnBoltDissipate = delegate { };

        #region Bolt Pool

        private List<BoltController> _boltControllers = new List<BoltController>();
        private BoltController _currentBolt;

        public void OnEnable()
        {
            if (_boltPrefab == null) {
                throw new MissingFieldException("Missing Bolt Prefab Reference on " + gameObject);
            }
            for (int i = _boltControllers.Count; i < _initialPoolSize; ++i) {
                CreateNewBolt();
            }
            UIEvents.current.OnPlayerRespawn += OnPlayerRespawn;
        }

        private void OnDisable()
        {
            if (UIEvents.current != null)
                UIEvents.current.OnPlayerRespawn -= OnPlayerRespawn;
        }

        public void AddController(BoltController controller)
        {
            if (_boltControllers.Contains(controller)) {
                if (_currentBolt == controller) {
                    _currentBolt = null;
                }
                return;
            }
            _boltControllers.Add(controller);
            controller.gameObject.SetActive(false);
        }

        private void GetNewBolt()
        {
            if (_currentBolt != null) {
                _currentBolt.Dissipate(false);
                _currentBolt = null;
            }
            if (_boltControllers.Count == 0) {
                CreateNewBolt();
            }
            BoltController controller = _boltControllers[0];
            _boltControllers.Remove(controller);
            _currentBolt = controller;
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

        public void OnPlayerRespawn()
        {
            if (_currentBolt != null) {
                _currentBolt.Disable();
            }
            _currentBolt = null;
            _isCasting = false;
            OnBoltDissipate?.Invoke(ResidueReady);
        }

        public void OnGamePaused()
        {
            if (_currentBolt == null) return;
            _currentBolt.Disable();
            _currentBolt = null;
            _isCasting = false;
        }

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
            OnResidueReady?.Invoke();
        }

        public void DissipateBolt()
        {
            if (_isCasting) return;
            _currentBolt = null;
            OnBoltDissipate?.Invoke(ResidueReady);
        }

        #endregion

        #region Manager To Bolt

        public void PrepareToFire(Vector3 position, Vector3 forward, bool isResidue)
        {
            GetNewBolt();
            _isCasting = true;
            _currentBolt.PrepareToFire(position, forward, isResidue);
        }

        public void SetPosition(Vector3 position, Vector3 forward)
        {
            if (!_isCasting) return;
            _currentBolt.SetPosition(position, forward);
        }

        public void SetCastStatus(float size)
        {
            if (!_isCasting) return;
            _currentBolt.SetCastStatus(size);
        }

        public void Fire(Vector3 position, Vector3 forward)
        {
            if (!_isCasting) return;
            _currentBolt.Fire(position, forward);
            _isCasting = false;
        }

        public void RedirectBolt(Vector3 position, Quaternion rotation, float timer)
        {
            if (_currentBolt == null) {
                GetNewBolt();
            }
            _currentBolt.Redirect(position, rotation, timer);
        }

        public bool PrepareToWarp()
        {
            if (_currentBolt == null || _residueInteractable != null) return false;
            return _currentBolt.PrepareToWarp();
        }

        public void OnWarp()
        {
            if (_currentBolt == null) return;
            _currentBolt.OnWarp();
        }

        public void DisableResidue()
        {
            _residueInteractable?.OnDisableWarpResidue();
            _residueInteractable = null;
        }

        #endregion
    }
}