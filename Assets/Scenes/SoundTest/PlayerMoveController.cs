using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour, PlayerInputAction.IFpsActions
{
    private CharacterController _characterController;
    private PlayerInputAction _inputAction;
    private Vector2 _moveActionValue;

    [SerializeField] private float characterMoveSpeed = 10.0f;

    [SerializeField] private AudioSource walkSound;
    
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        walkSound.Play();
    }

    void Update()
    {
        var verticalVector =
            transform.forward * (_moveActionValue.y * characterMoveSpeed * Time.deltaTime);
        var horizontalVector =
            transform.right * (_moveActionValue.x * characterMoveSpeed * Time.deltaTime);
        _characterController.Move(verticalVector + horizontalVector);
        
        if (_characterController.velocity.magnitude > 2.0f &&
            walkSound.isPlaying == false)
        {
            walkSound.pitch = Random.Range(0.8f, 1.1f);
            walkSound.Play();
        }
    }

    void OnEnable()
    {
        if(_inputAction == null)
        _inputAction = new PlayerInputAction();
        
        _inputAction.Fps.SetCallbacks(this);
        _inputAction.Fps.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveActionValue = context.ReadValue<Vector2>();
    }
}
