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
    public static bool UIWallGripPressed;
    public static bool UIDashPressed;
    public static bool UIMoveUp;


    public void OnMoveLeftDown() { UIMoveLeft = true; }
    public void OnMoveLeftUp() { UIMoveLeft = false; }
    public void OnMoveRightDown() { UIMoveRight = true; }
    public void OnMoveRightUp() { UIMoveRight = false; }
    public void OnJumpDown() { UIJumpPressed = true; }
    public void OnJumpUp() { UIJumpPressed = false; }
    public void OnWallGripDown() { UIWallGripPressed = true;}
    public void OnWallGripUp() { UIWallGripPressed = false;}
    public void OnDashDown() { UIDashPressed = true;}
    public void OnDashUp() {UIDashPressed = false;}
    public void OnMoveUpPressedDown() { UIMoveUp = true;}
    public void OnMoveUpPressedUp() { UIMoveUp = false;}



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
    private static bool _uiDashPrevFrame = false;

    private void Update()
    {
        Vector2 inputMovement = _moveAction.ReadValue<Vector2>();
        float x = inputMovement.x;
        float y = inputMovement.y;
        if (UIMoveLeft && !UIMoveRight) x = -1;
        if (UIMoveRight && !UIMoveLeft) x = 1;
        if (UIMoveLeft && UIMoveRight) x = 0; // both pressed, no movement
        if (UIMoveUp) y = 1;
        Movement = new Vector2(x, y);

        // UI jump "was pressed this frame" logic
        bool uiJumpWasPressedThisFrame = UIJumpPressed && !_uiJumpPrevFrame;
        _uiJumpPrevFrame = UIJumpPressed;

        // UI dash "was pressed this frame" logic
        bool uiDashWasPressedThisFrame = UIDashPressed && !_uiDashPrevFrame;
        _uiDashPrevFrame = UIDashPressed;

        JumpWasPressed = _jumpAction.WasPressedThisFrame() || uiJumpWasPressedThisFrame;
        JumpIsHeld = _jumpAction.IsPressed() || UIJumpPressed;
        JumpWasReleased = _jumpAction.WasReleasedThisFrame() || (!UIJumpPressed && _uiJumpPrevFrame);

        RunIsHeld = _runAction.IsPressed();
        DashWasPressed = _dashAction.WasPressedThisFrame() || uiDashWasPressedThisFrame;
    }
}
