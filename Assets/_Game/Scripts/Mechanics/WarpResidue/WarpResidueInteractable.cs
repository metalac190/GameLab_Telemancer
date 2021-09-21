using Mechanics.WarpBolt;
using UnityEngine;

namespace Mechanics.WarpResidue
{
    [RequireComponent(typeof(ResidueEffect))]
    public class WarpResidueInteractable : MonoBehaviour, IWarpInteractable
    {
        private ResidueEffect _residueEffect;

        private void Awake()
        {
            _residueEffect = GetComponent<ResidueEffect>();
            _residueEffect.enabled = false;
        }

        public bool OnWarpBoltImpact(BoltData data)
        {
            return true;
        }

        public bool OnSetWarpResidue(BoltData data)
        {
            _residueEffect.enabled = true;
            return true;
        }

        public void OnActivateWarpResidue(BoltData data)
        {
            _residueEffect.enabled = false;
        }
    }
}