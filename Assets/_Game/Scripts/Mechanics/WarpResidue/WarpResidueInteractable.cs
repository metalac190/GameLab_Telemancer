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

        public virtual bool OnWarpBoltImpact(BoltData data)
        {
            return true;
        }

        public virtual bool OnSetWarpResidue(BoltData data)
        {
            _residueEffect.enabled = true;
            return true;
        }

        public virtual void OnActivateWarpResidue(BoltData data)
        {
            _residueEffect.enabled = false;
        }

        public virtual void OnDisableWarpResidue()
        {
            _residueEffect.enabled = false;
        }
    }
}