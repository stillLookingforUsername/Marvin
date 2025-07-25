using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;
    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;
    public static bool DashWasPressed;

    // UI button support
    public static bool UIMoveLeft;
    public static bool UIMoveRight;
    public static bool UIJumpPressed;

    public void OnMoveLeftDown() { UIMoveLeft = true; }
    public void OnMoveLeftUp() { UIMoveLeft = false; }
    public void OnMoveRightDown() { UIMoveRight = true; }
    public void OnMoveRightUp() { UIMoveRight = false; }
    public void OnJumpDown() { UIJumpPressed = true; }
    public void OnJumpUp() { UIJumpPressed = false; }


    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _dashAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
        _dashAction = PlayerInput.actions["Dash"];
    }

    private static bool _uiJumpPrevFrame = false;

    private void Update()
    {
        Vector2 inputMovement = _moveAction.ReadValue<Vector2>();
        float x = inputMovement.x;
        if (UIMoveLeft && !UIMoveRight) x = -1;
        if (UIMoveRight && !UIMoveLeft) x = 1;
        if (UIMoveLeft && UIMoveRight) x = 0; // both pressed, no movement
        Movement = new Vector2(x, 0);

        // UI jump "was pressed this frame" logic
        bool uiJumpWasPressedThisFrame = UIJumpPressed && !_uiJumpPrevFrame;
        _uiJumpPrevFrame = UIJumpPressed;

        JumpWasPressed = _jumpAction.WasPressedThisFrame() || uiJumpWasPressedThisFrame;
        JumpIsHeld = _jumpAction.IsPressed() || UIJumpPressed;
        JumpWasReleased = _jumpAction.WasReleasedThisFrame() || (!UIJumpPressed && _uiJumpPrevFrame);

        RunIsHeld = _runAction.IsPressed();
        DashWasPressed = _dashAction.WasPressedThisFrame();
    }
}
