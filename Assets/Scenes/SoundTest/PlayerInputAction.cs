// GENERATED AUTOMATICALLY FROM 'Assets/Scenes/SoundTest/PlayerInputAction.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInputAction : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputAction"",
    ""maps"": [
        {
            ""name"": ""Fps"",
            ""id"": ""5aea1094-a0c1-424a-9290-f499d893686e"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""79482c47-c8d5-4477-aea4-095c26b79dd5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""796b4686-0ab0-4721-a189-8ea4abd7078c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""cef6d2ab-63fd-42b4-ad9f-b9708e2e1ba2"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f7864242-1d66-4479-bb3d-11a8ef8a7b8a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""37b60f46-f10e-435e-ac07-c164295c882c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ca9a5858-b7f2-4eb6-a46a-e1026ca6f651"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""FpsCamera"",
            ""id"": ""d86fbe72-4a8a-47ec-b11d-6e3610775f1b"",
            ""actions"": [
                {
                    ""name"": ""Aim"",
                    ""type"": ""PassThrough"",
                    ""id"": ""24b24d1b-f20d-4c56-8d22-ff9ad5aff46f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4831b016-feba-43b1-b0a1-15fd0d12bb7c"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Fps
        m_Fps = asset.FindActionMap("Fps", throwIfNotFound: true);
        m_Fps_Move = m_Fps.FindAction("Move", throwIfNotFound: true);
        // FpsCamera
        m_FpsCamera = asset.FindActionMap("FpsCamera", throwIfNotFound: true);
        m_FpsCamera_Aim = m_FpsCamera.FindAction("Aim", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Fps
    private readonly InputActionMap m_Fps;
    private IFpsActions m_FpsActionsCallbackInterface;
    private readonly InputAction m_Fps_Move;
    public struct FpsActions
    {
        private @PlayerInputAction m_Wrapper;
        public FpsActions(@PlayerInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Fps_Move;
        public InputActionMap Get() { return m_Wrapper.m_Fps; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FpsActions set) { return set.Get(); }
        public void SetCallbacks(IFpsActions instance)
        {
            if (m_Wrapper.m_FpsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_FpsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_FpsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_FpsActionsCallbackInterface.OnMove;
            }
            m_Wrapper.m_FpsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
            }
        }
    }
    public FpsActions @Fps => new FpsActions(this);

    // FpsCamera
    private readonly InputActionMap m_FpsCamera;
    private IFpsCameraActions m_FpsCameraActionsCallbackInterface;
    private readonly InputAction m_FpsCamera_Aim;
    public struct FpsCameraActions
    {
        private @PlayerInputAction m_Wrapper;
        public FpsCameraActions(@PlayerInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Aim => m_Wrapper.m_FpsCamera_Aim;
        public InputActionMap Get() { return m_Wrapper.m_FpsCamera; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FpsCameraActions set) { return set.Get(); }
        public void SetCallbacks(IFpsCameraActions instance)
        {
            if (m_Wrapper.m_FpsCameraActionsCallbackInterface != null)
            {
                @Aim.started -= m_Wrapper.m_FpsCameraActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_FpsCameraActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_FpsCameraActionsCallbackInterface.OnAim;
            }
            m_Wrapper.m_FpsCameraActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
            }
        }
    }
    public FpsCameraActions @FpsCamera => new FpsCameraActions(this);
    public interface IFpsActions
    {
        void OnMove(InputAction.CallbackContext context);
    }
    public interface IFpsCameraActions
    {
        void OnAim(InputAction.CallbackContext context);
    }
}
