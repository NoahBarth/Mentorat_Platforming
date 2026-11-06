using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class InputHandler : MonoBehaviour
{
    public PlayerController CharacterController;
    public CameraController CameraController;
    private InputAction moveAction, lookAction, jumpAction, resetAction;

    [Header("Gamepad Settings")]
    [SerializeField] private float gamepadLookSensitivity = 400f;
    [SerializeField] private float gamepadDeadzone = 0.15f;
    private bool usingGamepad = false;




    // Tunable jump parameters.
    public float JumpForce = 15f;
    public float JumpCutMultiplier = 0.35f;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");
        resetAction = InputSystem.actions.FindAction("Reset");

        jumpAction.started += OnJumpStarted;
        jumpAction.canceled += OnJumpCanceled;
        resetAction.started += OnResetStarted;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDestroy()
    {
        jumpAction.started -= OnJumpStarted;
        jumpAction.canceled -= OnJumpCanceled;
        resetAction.started -= OnResetStarted;
    }


    void Update()
    {
        moveCharacter();
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        CharacterController.Jump(JumpForce);
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        CharacterController.CutJump(JumpCutMultiplier);
    }

    private void OnResetStarted(InputAction.CallbackContext context)
    {
        SceneResetter.ResetScene();
    }

    private void moveCharacter()
    {
        // detect whether a gamepad is present this frame
        usingGamepad = Gamepad.current != null;

        // Movement: prefer action vector (works for both keyboard/gamepad bindings),
        // but fall back to direct gamepad stick if action is empty.
        Vector2 movementVector = moveAction.ReadValue<Vector2>();
        if (usingGamepad && movementVector.sqrMagnitude < 0.0001f)
        {
            movementVector = Gamepad.current.leftStick.ReadValue();
            // apply deadzone
            if (movementVector.magnitude < gamepadDeadzone) movementVector = Vector2.zero;
        }

        CharacterController.Move(movementVector);

        // Look: mouse look should come from the Look action. For gamepad, read right stick and
        // scale by a sensitivity and Time.deltaTime so it feels consistent.
        Vector2 lookVector = lookAction.ReadValue<Vector2>();

        if (usingGamepad)
        {
            Vector2 stick = Gamepad.current.rightStick.ReadValue();
            if (stick.magnitude < gamepadDeadzone) stick = Vector2.zero;
            // scale gamepad stick to match mouse-like input expected by the camera
            lookVector = stick * gamepadLookSensitivity * Time.deltaTime;
        }

        if (CameraController != null)
        {
            CameraController.RotateCamera(lookVector);
        }
    }
}
