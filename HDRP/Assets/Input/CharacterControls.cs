// GENERATED AUTOMATICALLY FROM 'Assets/Input/CharacterControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @CharacterControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @CharacterControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CharacterControls"",
    ""maps"": [
        {
            ""name"": ""DefaultMap"",
            ""id"": ""7f5c9eb4-425f-444c-8480-63169e3e7c0b"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""3ae5c18d-5d63-4c3c-9acb-a8aa491ed2f9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""ebad2b5c-f202-4d56-8b5d-42db878c76c8"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""a8a287a7-33a1-4486-b858-5927d7399216"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""b0c14907-f365-43c0-84e5-e84cfc5d3621"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""c0e7e1ed-3a71-4509-b462-1d4eaf007e0c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""77c5f64c-9359-4097-bc16-e80cab15bda1"",
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
                    ""id"": ""b63c9670-9743-4057-a854-425095cdb85c"",
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
                    ""id"": ""eef9811b-627d-444f-8fbf-1847581b1d30"",
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
                    ""id"": ""cf65551b-04f1-4942-abed-a410ac8ab07f"",
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
                    ""id"": ""23f35af5-6ce9-49e1-b662-6fc5c29c2b77"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""3cafa599-1630-44ef-bf6b-b66ffc67acca"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6f542c8a-52cc-4a8d-aef0-9ab799873dcc"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b8d68b9f-827a-4dd4-9c07-b2c696ea0f85"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b216bbb5-ce65-4036-8acc-e34a9e3935aa"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard+Mouse"",
            ""bindingGroup"": ""Keyboard+Mouse"",
            ""devices"": []
        }
    ]
}");
        // DefaultMap
        m_DefaultMap = asset.FindActionMap("DefaultMap", throwIfNotFound: true);
        m_DefaultMap_Move = m_DefaultMap.FindAction("Move", throwIfNotFound: true);
        m_DefaultMap_Look = m_DefaultMap.FindAction("Look", throwIfNotFound: true);
        m_DefaultMap_Jump = m_DefaultMap.FindAction("Jump", throwIfNotFound: true);
        m_DefaultMap_Sprint = m_DefaultMap.FindAction("Sprint", throwIfNotFound: true);
        m_DefaultMap_Crouch = m_DefaultMap.FindAction("Crouch", throwIfNotFound: true);
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

    // DefaultMap
    private readonly InputActionMap m_DefaultMap;
    private IDefaultMapActions m_DefaultMapActionsCallbackInterface;
    private readonly InputAction m_DefaultMap_Move;
    private readonly InputAction m_DefaultMap_Look;
    private readonly InputAction m_DefaultMap_Jump;
    private readonly InputAction m_DefaultMap_Sprint;
    private readonly InputAction m_DefaultMap_Crouch;
    public struct DefaultMapActions
    {
        private @CharacterControls m_Wrapper;
        public DefaultMapActions(@CharacterControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_DefaultMap_Move;
        public InputAction @Look => m_Wrapper.m_DefaultMap_Look;
        public InputAction @Jump => m_Wrapper.m_DefaultMap_Jump;
        public InputAction @Sprint => m_Wrapper.m_DefaultMap_Sprint;
        public InputAction @Crouch => m_Wrapper.m_DefaultMap_Crouch;
        public InputActionMap Get() { return m_Wrapper.m_DefaultMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultMapActions set) { return set.Get(); }
        public void SetCallbacks(IDefaultMapActions instance)
        {
            if (m_Wrapper.m_DefaultMapActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnMove;
                @Look.started -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnLook;
                @Jump.started -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnJump;
                @Sprint.started -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnSprint;
                @Crouch.started -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_DefaultMapActionsCallbackInterface.OnCrouch;
            }
            m_Wrapper.m_DefaultMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
            }
        }
    }
    public DefaultMapActions @DefaultMap => new DefaultMapActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard+Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IDefaultMapActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
    }
}
