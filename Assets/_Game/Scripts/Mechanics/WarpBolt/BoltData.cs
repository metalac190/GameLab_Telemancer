using UnityEngine;

namespace Mechanics.WarpBolt
{
    /// Summary:
    /// The Bolt Data Scriptable Object that is used by the Bolt Controller
    /// This scripts main purpose is to hold data such as the Bolt Controller and the Player Controller
    /// It is passed through the OnWarpBoltImpact function
    public class BoltData : ScriptableObject
    {
        private BoltController _warpBolt;
        private PlayerController _playerController;

        #region Reference Properties and Setters

        public BoltController WarpBolt
        {
            get
            {
                if (_warpBolt == null) {
                    _warpBolt = FindObjectOfType<BoltController>();
                    if (_warpBolt == null) {
                        Debug.LogError("Cannot Find Bolt Controller", this);
                    }
                }
                return _warpBolt;
            }
            private set => _warpBolt = value;
        }

        public PlayerController PlayerController
        {
            get
            {
                if (_playerController == null) {
                    _playerController = FindObjectOfType<PlayerController>();
                    if (_playerController == null) {
                        Debug.LogError("Cannot Find Player Controller", this);
                    }
                }
                return _playerController;
            }
            private set => _playerController = value;
        }

        public void SetWarpBoltReference(BoltController warpBoltController)
        {
            WarpBolt = warpBoltController;
        }

        public void SetPlayerReference(PlayerController playerController)
        {
            PlayerController = playerController;
        }

        #endregion


        // The direction that the warp bolt is traveling in. Can be modified by the ResetDirection() function below
        public Vector3 Direction { get; private set; } = Vector3.zero;

        public void ResetDirection(Vector3 direction)
        {
            Direction = direction;
        }
    }
}