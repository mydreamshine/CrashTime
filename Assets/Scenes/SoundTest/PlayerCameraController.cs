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
    
    private float cameraShakeStrength = 0.1f;
    private float cameraShakeDuration = 1.0f;
    private float cameraShakeTime = 0.0f;
    private Vector3 originCameraPosition;

    [SerializeField] private AudioSource shootSound;
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

        if (true == _isShooting)
        {
            var randomPos = Random.insideUnitCircle * cameraShakeStrength;
            cameraTransform.localPosition =
                originCameraPosition + new Vector3(randomPos.x, randomPos.y, 0);
            cameraShakeTime += Time.deltaTime * cameraShakeStrength;
            if (cameraShakeTime > cameraShakeDuration)
            {
                cameraTransform.localPosition = originCameraPosition;
                cameraShakeTime = 0.0f;
                _isShooting = false;
            }
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
        
        if (false == shootSound.isPlaying)
        {
            shootSound.pitch = Random.Range(0.8f, 1.1f);
            shootSound.Play();
        }
    }
}
