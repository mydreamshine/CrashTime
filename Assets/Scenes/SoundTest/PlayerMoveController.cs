using KPU;
using KPU.Manager;
using Scenes.PlayScenes.SlowMotionFunc;
using Scenes.SharedDataEachScenes;
using Scenes.SharedDataEachScenes.Prefabs.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour, PlayerInputAction.IFpsActions
{
    private CharacterController _characterController;
    private PlayerInputAction _inputAction;
    private Vector2 _moveActionValue;

    [SerializeField] public float characterMoveSpeed = 10.0f;

    [SerializeField] private AudioSource walkSound;

    [SerializeField] private OptionsButtonUI optionsButtonUI;

    private SlowMotionController slowMotionController;
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        walkSound.Play();

        slowMotionController = FindObjectOfType<SlowMotionController>();
    }

    void Update()
    {
        // Skinned Mesh Destroy의 Matrix Operating에 의한 deltaTime Delay 일시적 보완
        if (1 / Time.unscaledDeltaTime < 20.0f) return;
        
        if (GameManager.Instance.State == State.Paused) return;

        var slowModeDeltaTime = Mathf.Clamp(SlowMotionManager.Instance.CurrentSlowSpeed, 0.5f, 1.0f);
        slowModeDeltaTime *= Time.deltaTime;

        var verticalVector =
            transform.forward * (_moveActionValue.y * characterMoveSpeed * slowModeDeltaTime);
        var horizontalVector =
            transform.right * (_moveActionValue.x * characterMoveSpeed * slowModeDeltaTime);
        _characterController.Move(verticalVector + horizontalVector);

        if (slowMotionController != null)
        {
            var irregularRangeValue = slowMotionController.SlowScale;
            var irregularMin = irregularRangeValue - irregularRangeValue * 0.3f;
            var irregularMax = irregularRangeValue + irregularRangeValue * 0.3f;
            irregularRangeValue = Random.Range(irregularMin, irregularMax);
            walkSound.pitch = Mathf.Clamp(irregularRangeValue, 0.1f, 1.0f);
        }

        if (_characterController.velocity.magnitude > 2.0f &&
            walkSound.isPlaying == false)
        {
            walkSound.Play();
        }

        // if (Input.GetKeyDown(KeyCode.Tab))
        // {
        //     if (GameManager.Instance.State == State.Paused) return;
        //     GameManager.Instance.SetState(State.Paused);
        //     EventManager.Emit("game_pause");
        //     EventManager.Emit("open_option_panel");
        // }
    }

    void OnEnable()
    {
        if(_inputAction == null) _inputAction = new PlayerInputAction();
        
        _inputAction.Fps.SetCallbacks(this);
        _inputAction.Fps.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveActionValue = context.ReadValue<Vector2>();
    }
}
