using UnityEngine;
using System;
using AudioSystem;
using Mechanics.Bolt;
using Mechanics.Player.Feedback.Options;

/// Summary:
/// Temporary Large / Big Rock Script. Mainly for reference and testing
/// Inherits from Warp Residue Interactable, and overrides the WarpBoltImpact() to swap itself with the player
public class LargeRock : WarpResidueInteractable
{
    // A simple offset for the teleportation location
    [SerializeField] private Vector3 _teleportOffset = Vector3.up;
    // the position of the object on start
    private Vector3 _spawnPos = new Vector3();
    private Quaternion _spawnRot = new Quaternion();
    private PlayerOptionsController _codeController;

    [Header("Rock is Ted")]
    [SerializeField] private GameObject _rockModel = null;
    [SerializeField] private GameObject _tedModel = null;

    // if rock is Max distance away from its start in any direction, reset it
    [Header("Position Thresholds")]
    [SerializeField] private float MaxX = 100;
    [SerializeField] private float MaxY = 100;
    [SerializeField] private float MaxZ = 100;

    [Header("Audio")]
    [SerializeField] private SFXOneShot _impactSound = null;
    [SerializeField] private float timeToWaitForSound = .5f;

    private Rigidbody _rb = null;


    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _spawnPos = transform.position;
        _spawnRot = transform.rotation;
        //OnTedRocks(false);

        // Ted Rocks
        _codeController = GetCodeController();
        UIEvents.current.OnOpenCodeMenu += UpdateCodeController;
        if (PlayerPrefs.GetInt("TedRocks") == 1)
            OnTedRocks(true);
    }

    private void OnEnable()
    {
        UIEvents.current.OnPlayerRespawn += Reset;
    }

    private void OnDisable()
    {
        if(UIEvents.current != null)
            UIEvents.current.OnPlayerRespawn -= Reset;

        if (_codeController != null)
            _codeController.ImprovedWaterfalls.OnSelect -= OnTedRocks;
    }

    private void Update()
    {
        Vector3 distance = transform.position - _spawnPos;
        if (Mathf.Abs(distance.x) > MaxX || Mathf.Abs(distance.y) > MaxY || Mathf.Abs(distance.z) > MaxZ)
        {
            Reset();
            AchievementManager.current.unlockAchievement(AchievementManager.Achievements.RockOutOfBounds);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.timeSinceLevelLoad > timeToWaitForSound && collision.gameObject.layer == LayerMask.NameToLayer("Default"))
            _impactSound?.PlayOneShot(transform.position);
    }

    /// Called on either WarpBoltImpact or WarpResidueActivated, see WarpResidueInteractable
    public override bool OnWarpBoltImpact(BoltData data)
    {
        // On the player controller, teleport this transform with an offset
        data.PlayerController.Teleport(transform, _teleportOffset);

        // Boolean return value to determine whether to dissipate warp bolt after impact
        return true;
    }

    // The other 3 functions can be inherited from for extra capabilities

    public void Reset()
    {
        _rb.velocity = new Vector3(0, 0, 0);
        transform.position = _spawnPos;
        transform.rotation = _spawnRot;
    }

    private void OnTedRocks(bool status)
    {
        if(_tedModel != null)
            _tedModel?.SetActive(status);
        if(_rockModel != null)
            _rockModel?.SetActive(!status);
        PlayerPrefs.SetInt("TedRocks", status ? 1 : 0);
    }

    private PlayerOptionsController GetCodeController()
    {
        PlayerOptionsController controller = PlayerOptionsController.Instance;
        if (controller == null)
            controller = FindObjectOfType<PlayerOptionsController>();
        if (controller != null)
            controller.TedRocks.SelectItem(PlayerPrefs.GetInt("TedRocks") == 1 ? true : false);
        return controller;
    }

    private void UpdateCodeController()
    {
        if (_codeController == null)
            _codeController = GetCodeController();
        _codeController.TedRocks.OnSelect += OnTedRocks;
        UIEvents.current.OnOpenCodeMenu -= UpdateCodeController;
    }
}