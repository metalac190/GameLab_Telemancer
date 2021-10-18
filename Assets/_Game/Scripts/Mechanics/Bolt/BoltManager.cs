using System;
using System.Collections.Generic;
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

        private IWarpInteractable _residueInteractable = null;

        public BoltController GetBolt => _currentBolt;
        public bool CanWarp => _currentBolt != null;
        public bool ResidueReady => _currentBolt != null;
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
        }

        public void AddController(BoltController controller)
        {
            if (_boltControllers.Contains(controller)) return;
            _boltControllers.Add(controller);
            controller.gameObject.SetActive(false);
        }

        private void GetNewBolt()
        {
            if (_currentBolt != null) {
                _currentBolt.Dissipate(false, false);
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
            OnBoltDissipate?.Invoke(ResidueReady);
        }

        #endregion

        #region Manager To Bolt

        public void PrepareToFire(Vector3 position, Vector3 forward, bool isResidue)
        {
            GetNewBolt();

            _currentBolt.PrepareToFire(position, forward, isResidue);
        }

        public void SetPosition(Vector3 position, Vector3 forward)
        {
            _currentBolt.SetPosition(position, forward);
        }

        public void SetCastStatus(float size)
        {
            _currentBolt.SetCastStatus(size);
        }

        public void Fire(Vector3 position, Vector3 forward)
        {
            _currentBolt.Fire(position, forward);
        }

        public bool OnWarp()
        {
            return _currentBolt != null && _currentBolt.OnWarp();
        }

        public void DisableResidue()
        {
            _residueInteractable?.OnDisableWarpResidue();
            _residueInteractable = null;
        }

        public void Dissipate()
        {
            if (_currentBolt == null) return;
            _currentBolt.Dissipate(false, false);
            _currentBolt = null;
        }

        #endregion
    }
}