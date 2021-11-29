using System.Collections;
using System.Collections.Generic;
using Mechanics.Bolt;
using UnityEngine;

public class RelayStone : WarpResidueInteractable
{
    [Header("Relay Stone")]
    [SerializeField] private RelayStone _relayPair = null;
    [SerializeField] private GameObject _boltSource = null;
    [Header("VFX")]
    [SerializeField] private bool vfxActive = true;
    [SerializeField] private GameObject _vfxBeam = null;
    [SerializeField] private GameObject _vfxEndPoint = null;
    [SerializeField] private GameObject _vfxTarget = null;

    [Header("Debuging")]
    [SerializeField] private float _trajectoryRayGizmo = 5;

    public override bool OnWarpBoltImpact(BoltData data)
    {
        // Redirect the warp bolt
        // adding some value to transform.position so that the bolt doesn't spawn inside the other relay stone and immediately collide

        data.BoltManager.RedirectBolt(_relayPair.gameObject, _relayPair._boltSource.transform.position, _relayPair._boltSource.transform.rotation, 0);
        StartCoroutine(_relayPair.IgnoreCollisionWithBolt(data));
        //StartCoroutine(IgnoreCollisionWithBolt(data));
        // Don't dissipate the warp bolt!
        return false;
    }

    public override bool OnSetWarpResidue(BoltData data)
    {
        _relayPair._vfxBeam.SetActive(false);
        _vfxBeam.SetActive(true);
        return base.OnSetWarpResidue(data);
    }

    public override bool DoesResidueReturnToHoldAnimation()
    {
        return true;
    }

    public IEnumerator IgnoreCollisionWithBolt(BoltData data)
    {
        BoltController currentBolt = data.BoltManager.CurrentBolt;
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), currentBolt.Collider, true);
        yield return new WaitForSecondsRealtime(0.25f);
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), currentBolt.Collider, false);
    }

    private void FixedUpdate()
    {
        if (_relayPair != null)
        {
            _vfxEndPoint.transform.position = _relayPair._vfxTarget.transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (_relayPair != null)
        {
            Gizmos.DrawLine(transform.position, _relayPair.transform.position);
            _vfxEndPoint.transform.position = _relayPair._vfxTarget.transform.position;
        }

        Gizmos.color = Color.red;
        Vector3 direction = _boltSource.transform.TransformDirection(Vector3.forward * _trajectoryRayGizmo);
        Gizmos.DrawRay(_boltSource.transform.position, direction);

        if(!vfxActive && _vfxBeam.activeSelf)
        {
            _vfxBeam.SetActive(false);
        }
        else if(vfxActive && !_vfxBeam.activeSelf)
        {
            _vfxBeam.SetActive(true);
        }
    }
}