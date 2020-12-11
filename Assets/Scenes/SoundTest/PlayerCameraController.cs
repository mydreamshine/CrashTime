using System;
using System.Collections;
using System.Collections.Generic;
using KPU;
using KPU.Manager;
using Scenes.PlayScenes.SlowMotionFunc;
using Scenes.SharedDataEachScenes;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerCameraController : MonoBehaviour,PlayerInputAction.IFpsCameraActions
{
    private PlayerInputAction _inputAction;
    private Vector2 _mouseDeltaVector;
    private float _cameraRotation;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 100.0f;
    [SerializeField] private Transform lookingTargetTransform;
    
    private float cameraShakeStrength = 1.0f;
    private float cameraShakeDuration = 1.0f;
    private float cameraShakeTime = 0.0f;
    private Vector3 originCameraPosition;
    
    private float shootedTime = 0.0f;

    [SerializeField] private Transform rifleTransform;
    public float verticalRecoil;
    public float horizontalRecoil;
    private float stackedVertical = 0.0f;
    private float stackedHorizontal = 0.0f;
    private Quaternion releasedCameraRotation;
    private float resetRotTimeStack;
    
    private GunControl gunControl;

    private void Awake()
    {
        gunControl = GetComponentInChildren<GunControl>();
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        originCameraPosition = cameraTransform.localPosition;

        if (gunControl != null)
        {
            gunControl.IsExistRebound(true);
            cameraShakeDuration = gunControl.shootDelay;
        }
    }
    void Update()
    {
        // Skinned Mesh Destroy의 Matrix Operating에 의한 deltaTime Delay 일시적 보완
        if (1 / Time.unscaledDeltaTime < 20.0f) return;
        
        if (GameManager.Instance.State == State.Paused) return;
        
        var slowModeDeltaTime = Mathf.Clamp(SlowMotionManager.Instance.CurrentSlowSpeed, 0.5f, 1.0f);
        slowModeDeltaTime *= Time.deltaTime;
        
        var horizontalDirection = 
            _mouseDeltaVector.x * slowModeDeltaTime * mouseSensitivity;
        transform.Rotate(0,horizontalDirection,0);

        
        _cameraRotation = Mathf.Clamp(
            _cameraRotation - _mouseDeltaVector.y * slowModeDeltaTime * mouseSensitivity,
            -90.0f, 90.0f);
        cameraTransform.localRotation = Quaternion.Euler(_cameraRotation,0,0);
        lookingTargetTransform.rotation = transform.rotation;
        lookingTargetTransform.position = rifleTransform.position + cameraTransform.forward* 10.0f;

        if (gunControl == null) return;
        if (gunControl.isShooting)
        {
            if (!gunControl.checkShootingDelayDone)
            {
                //var randomPos = Random.insideUnitCircle * cameraShakeStrength;
                //cameraTransform.localPosition =
                //    originCameraPosition + new Vector3(randomPos.x, randomPos.y, 0);
                cameraShakeTime += slowModeDeltaTime * cameraShakeStrength;
                
                AddRecoil();
                
                if (cameraShakeTime > cameraShakeDuration)
                {
                    cameraShakeTime = 0.0f;
                    cameraTransform.localPosition = originCameraPosition;
                    releasedCameraRotation = cameraTransform.localRotation;
                    StopRecoil();
                    gunControl.ResetRebound();
                }
            }
        }
        else
        {
            cameraShakeTime = 0.0f;
            cameraTransform.localPosition = originCameraPosition;
            releasedCameraRotation = cameraTransform.localRotation;
            StopRecoil();
        }
    }
    
    void OnEnable()
    {
        if (_inputAction == null) _inputAction = new PlayerInputAction();
        
        _inputAction.FpsCamera.SetCallbacks((this));
        _inputAction.FpsCamera.Enable();
    }

    void OnDisable()
    {
        _inputAction.Disable();
    }
    
    public void OnAim(InputAction.CallbackContext context)
    {
        _mouseDeltaVector = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }
    
    private void AddRecoil()
    {
        var slowModeDeltaTime = Mathf.Clamp(SlowMotionManager.Instance.CurrentSlowSpeed, 0.5f, 1.0f);
        //slowModeDeltaTime *= Time.deltaTime;
        var deltaScale = (1.0f - (cameraShakeTime / cameraShakeDuration)) * slowModeDeltaTime;
        deltaScale = Mathf.Clamp(deltaScale, 0.0f, 1.0f);
        float v = Random.Range(0.0f, verticalRecoil) * deltaScale;
        float h = Random.Range(-horizontalRecoil, horizontalRecoil) * deltaScale;
        stackedVertical += v;
        stackedHorizontal += h;

        if (!(resetRotTimeStack - 0.0f < Mathf.Epsilon)) resetRotTimeStack = 0.0f;
        
        cameraTransform.Rotate(v, h,0 );
        rifleTransform.Rotate(v * 0.5f, h,0 );
    }

    private void StopRecoil()
    {
        //Quaternion dest = Quaternion.Euler(-stackedVertical, -stackedHorizontal, 0);
        //Quaternion.RotateTowards(cameraTransform.rotation, dest, Time.deltaTime);
        //Quaternion.RotateTowards(rifleTransform.rotation, dest, Time.deltaTime);
        //cameraTransform.Rotate(-stackedVertical, -stackedHorizontal,0 );
        //rifleTransform.Rotate(-stackedVertical, -stackedHorizontal,0 );
        cameraTransform.localRotation = releasedCameraRotation;
        //rifleTransform.localRotation = Quaternion.identity;
        rifleTransform.localRotation = Quaternion.Slerp(rifleTransform.localRotation, Quaternion.identity, resetRotTimeStack);
        
        var slowModeDeltaTime = SlowMotionManager.Instance.CurrentSlowSpeed;
        slowModeDeltaTime *= (Time.deltaTime / cameraShakeDuration) * 9.0f;

        resetRotTimeStack = Mathf.Clamp(resetRotTimeStack + slowModeDeltaTime, 0.0f, 1.0f);
        stackedVertical = 0.0f;
        stackedHorizontal = 0.0f;
    }
}
