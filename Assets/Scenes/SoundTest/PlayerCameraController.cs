using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour,PlayerInputAction.IFpsCameraActions
{
    private PlayerInputAction _inputAction;
    private Vector2 _mouseDeltaVector;
    private float _cameraRotation;
    
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 100.0f;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
}
