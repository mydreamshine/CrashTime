using System;
using UnityEngine;
using changhoScript;
using KPU;
using KPU.Manager;
using Scenes.PlayScenes.SlowMotionFunc;
using Scenes.SharedDataEachScenes;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GunControl : MonoBehaviour, PlayerInputAction.IFpsCameraActions
{
    [SerializeField]
    private BulletObjectPool bulletPool;

    [SerializeField]
    private int bulletSpeed;

    [SerializeField] public float shootDelay = 0.39f;
    [SerializeField] private AudioSource shootSound;
    
    private SlowMotionController slowMotionController;
    private float shootingDelayTimeStack;
    
    [HideInInspector] public bool checkShootingDelayDone = true;
    [HideInInspector] public bool isShooting;
    private bool existRebound;

    private void Awake()
    {
        slowMotionController = FindObjectOfType<SlowMotionController>();
    }

    private void Update()
    {
        // Skinned Mesh Destroy의 Matrix Operating에 의한 deltaTime Delay 일시적 보완
        if (1 / Time.unscaledDeltaTime < 20.0f) return;
        
        // Set Shooting Sound Pitch
        if (slowMotionController != null)
        {
            var irregularRangeValue = slowMotionController.SlowScale;
            var irregularMin = irregularRangeValue - irregularRangeValue * 0.3f;
            var irregularMax = irregularRangeValue + irregularRangeValue * 0.3f;
            irregularRangeValue = Random.Range(irregularMin, irregularMax);
            shootSound.pitch = Mathf.Clamp(irregularRangeValue, 0.1f, 1.0f);
        }
        
        if (Input.GetMouseButtonDown(0) && checkShootingDelayDone)
        {
            if (GameManager.Instance.State == State.Playing) Fire();
        }

        if (!checkShootingDelayDone)
        {
            var slowModeDeltaTime = SlowMotionManager.Instance.CurrentSlowSpeed * Time.deltaTime;
            if (shootingDelayTimeStack >= shootDelay)
            {
                isShooting = false;
                checkShootingDelayDone = true;
                shootingDelayTimeStack = 0.0f;
            }
            else shootingDelayTimeStack += slowModeDeltaTime;
        }
    }

    private void Fire()
    {
        Manager.instance.MuzzlePaticleOn();
        shootSound.Stop();
        shootSound.Play();
        
        var clone = bulletPool.GetObject();
        clone.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);

        isShooting = true;
        checkShootingDelayDone = false;
        shootingDelayTimeStack = 0.0f;
    }

    public void IsExistRebound(bool exist)
    {
        existRebound = exist;
    }

    public void ResetRebound()
    {
        if (!existRebound) return;
        isShooting = false;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        isShooting = checkShootingDelayDone ? context.ReadValueAsButton() : isShooting;
    }
}
