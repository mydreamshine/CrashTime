using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour,PlayerInputAction.IFpsCameraActions
{
    private PlayerInputAction _inputAction;
    private Vector2 _mouseDeltaVector;
    private float _cameraRotation;
    private bool _isShooting;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 100.0f;
    [SerializeField] private Transform lookingTargetTransform;
    
    private float cameraShakeStrength = 0.1f;
    private float cameraShakeDuration = 1.0f;
    private float cameraShakeTime = 0.0f;
    private Vector3 originCameraPosition;

    [SerializeField] private AudioSource shootSound;
    [SerializeField] private float shootingDelay = 0.5f;
    private float shootedTime = 0.0f;
    
    [SerializeField] private Transform rifleTransform;
    public float verticalRecoil;
    public float horizontalRecoil;
    private float stackedVertical = 0.0f;
    private float stackedHorizontal = 0.0f;
    private Quaternion releasedCameraRotation;
    
    bool check = true;
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        originCameraPosition = cameraTransform.localPosition;
    }
    void Update()
    {
        var horizontalDirection = 
            _mouseDeltaVector.x * Time.deltaTime * mouseSensitivity;
        transform.Rotate(0,horizontalDirection,0);

        
        _cameraRotation = Mathf.Clamp(
            _cameraRotation - _mouseDeltaVector.y * Time.deltaTime * mouseSensitivity,
            -90.0f, 90.0f);
        cameraTransform.localRotation = Quaternion.Euler(_cameraRotation,0,0);
        lookingTargetTransform.rotation = transform.rotation;
        lookingTargetTransform.position = rifleTransform.position + cameraTransform.forward* 10.0f;

        if (true == _isShooting)
        {
            if (check == true)
            {
                check = false;
                if (false == shootSound.isPlaying)
                {
                    shootSound.pitch = Random.Range(0.8f, 1.1f);
                    shootSound.Play();
                }

                //var randomPos = Random.insideUnitCircle * cameraShakeStrength;
                //cameraTransform.localPosition =
                //    originCameraPosition + new Vector3(randomPos.x, randomPos.y, 0);
                cameraShakeTime += Time.deltaTime * cameraShakeStrength;
                if (cameraShakeTime > cameraShakeDuration)
                {
                    cameraTransform.localPosition = originCameraPosition;
                    cameraShakeTime = 0.0f;
                    _isShooting = false;
                }
                AddRecoil();
            }
            StartCoroutine(Wait());
        }
        else
        {
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
        _isShooting = context.ReadValueAsButton();
    }
    
    private void AddRecoil()
    {
        float v = Random.Range(0.0f, verticalRecoil);
        float h = Random.Range(-horizontalRecoil, horizontalRecoil);
        stackedVertical += v;
        stackedHorizontal += h;
        
        cameraTransform.Rotate(v, h,0 );
        rifleTransform.Rotate(v, h,0 );
    }

    private void StopRecoil()
    {
        Quaternion dest = Quaternion.Euler(-stackedVertical, -stackedHorizontal, 0);
        //Quaternion.RotateTowards(cameraTransform.rotation, dest, Time.deltaTime);
        //Quaternion.RotateTowards(rifleTransform.rotation, dest, Time.deltaTime);
        //cameraTransform.Rotate(-stackedVertical, -stackedHorizontal,0 );
        //rifleTransform.Rotate(-stackedVertical, -stackedHorizontal,0 );
        cameraTransform.localRotation = releasedCameraRotation;
        rifleTransform.localRotation = Quaternion.identity;
        stackedVertical = 0.0f;
        stackedHorizontal = 0.0f;
    }
    
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        check = true;

    }
}
