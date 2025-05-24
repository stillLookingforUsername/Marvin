using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    
    public Rigidbody2D rb;
    public bool isFacingRight = true;
    public Animator animator;
    public ParticleSystem dustCloud;

    [Header("Movement")]
    public float moveSpeed = 6f;
    private float horizontalMovement;

    [Header("Jumping")]
    public float jumpForce = 10f;
    public float hold = 0.5f;
    public int maxJump = 2;
    private int _jumpRemaning;
    private bool _grounded;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask groundLayer;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.5f);
    public LayerMask wallLayer;

    [Header("Wall Movement")]
    public float wallSlideSpeed = 5f;
    private bool _isWallSliding;

    [Header("Wall Jump")]
    public Vector2 wallJumpPower = new Vector2(8f, 12f);
    private bool _isWallJumping;
    public float wallJumpTime = 0.2f;
    private float wallJumpCounter;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 10f;
    public float fallSpeedMulti = 2f;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("trying to jump");
            animator.SetTrigger("jump");
        }
    }
    private void FixedUpdate()
    {
        _grounded = IsGrounded();

        if (_grounded)
            _jumpRemaning = maxJump;

        Gravity();
        WallSlide();
        WallJump();

        if (!_isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            Flip();
        }
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("isWallSliding", _isWallSliding);
    }

    private void JumpFx()
    {
        animator.SetTrigger("jump");
        dustCloud.Play();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && _isWallSliding)
        {
            JumpFx();
            _isWallJumping = true;
            wallJumpCounter = wallJumpTime;

            Vector2 dir = new Vector2(-Mathf.Sign(transform.localScale.x), 1).normalized;
            rb.linearVelocity = new Vector2(dir.x * wallJumpPower.x, dir.y * wallJumpPower.y);
        }
        else if (context.performed && _jumpRemaning > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            _jumpRemaning--;
            JumpFx();
        }
        else if (context.canceled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * hold);
        }
    }

    private void WallJump()
    {
        if (_isWallJumping)
        {
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter <= 0)
            {
                _isWallJumping = false;
            }
        }
    }

    private void WallSlide()
    {
        if (!_grounded && IsOnWall() && horizontalMovement != 0)
        {
            _isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            _isWallSliding = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }

    private bool IsOnWall()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMulti;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void Flip()
    {
        if (horizontalMovement > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalMovement < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (rb.linearVelocity.y == 0)
        {
            dustCloud.Play();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
