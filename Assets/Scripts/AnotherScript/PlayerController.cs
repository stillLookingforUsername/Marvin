using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    public PlayerData data;

    public float airDrag = 0.95f;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float wallHopForce;
    public float wallJumpForce;
    public float jumpBufferTime = 0.12f;
    public float turnTimerSet = 0.1f;
    public float variableJumpForce = 0.5f;
    public float groundCheckRadius;
    public float wallJumpTimerSet = 0.5f;
    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;


    /*public float XledgeClimbOffSet1 = 0f;
    public float YledgeClimbOffSet1 = 0f;
    public float XledgeClimbOffSet2 = 0f;
    public float YledgeClimbOffSet2 = 0f;
    */

    private float _playerInputDirection;
    private float _turnTimer;
    private float _jumpBufferTimer;
    private float _wallJumpTimer;
    private float _knockBackStartTime;
    [SerializeField] private float _knockBackDuration;

    [Header("Dash Setting")]
    private bool _isDashing;        //if the player is currently dashing or not
    public float dashTime;      //how long the dash should take
    public float dashSpeed;     //how fast the character should move while dashing
    public float distanceBetweenImages;     //how far apart the afterImage gameObject should be placed when dashing
    public float dashCoolDown;      //how we have to wait before we could dash again
    private float _dashTimeLeft;    //how much longer the dash should be happening
    private float _lastImageXpos;   // to keep track of last x-coordinate where we place an afterImage
    private float _lastDash = -100;        //keep track of last time we started a dash & will be used to check for the cooldown




    [SerializeField] private Vector2 _knockBackSpeed;
    private Vector2 _ledgePosBot;
    private Vector2 _ledgePos1;
    private Vector2 _ledgePos2;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;
    //public Vector2 ledgePosBottom;
    //public Vector2 ledgePos1;
    //public Vector2 ledgePos2;

    private Rigidbody2D _rb;
    private Animator _animator;

    public Transform groundCheckObjectTransform;
    public Transform wallCheck;
    public Transform ledgeCheck;

    public LayerMask groundLayer;
    public LayerMask wallLayer;


    private bool _isFacingRight = true;
    private bool _isRunning;
    private bool _isGrounded;
    private bool _isTouchingWall;
    private bool _isWallSliding;
    private bool _isAttemptingToJump;
    private bool _canWallJump;
    private bool _canNormalJump;
    private bool _checkJumpMultiplier;
    private bool _canMove;
    private bool _canFlip;
    private bool _hasWallJump;
    private bool _isTouchingLedge;
    private bool _canClimbLedge = false;
    private bool _ledgeDetected;
    private bool _knockBack;


    public int facingRightDirection = 1;

    private int _noOfJumpsLeft;
    private int _noOfJumps = 1;
    private int lastWallJumpDirection;

    public GameManager cm;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            cm.coinCnt++;
        }
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    private void Update()
    {
        CheckKnockBack();
        CheckInput();
        MovementDirection();
        UpdateAnimatrion();
        CheckIfCanJumpTrue();
        CheckWallSliding();
        WhichJumpFunction();
        CheckLedgeClimb();
        CheckDash();
        //CheckDash();
        //DoLedgeClimb();
        //FinishLedgeClimb();
    }

    private void FixedUpdate()
    {
        CheckSurroundings();
        ApplyMovement();
    }

    public bool GetDashCheck()
    {
        //this fun just return whether the payer is currently dashing or not
        return _isDashing;
    }
    private void UpdateAnimatrion()
    {
        _animator.SetBool("isRunningAnimatorParam", _isRunning);
        _animator.SetBool("isGroundedAnimatorParam", _isGrounded);
        _animator.SetFloat("yVelocityAnimatorParam", _rb.linearVelocity.y);
        _animator.SetBool("isWallSlidingAnimatorParam", _isWallSliding);
    }

    public void KnockBack(int direction)
    {
        _knockBack = true;
        _knockBackStartTime = Time.time;
        _rb.linearVelocity = new Vector2(_knockBackSpeed.x * direction, _knockBackSpeed.y);
    }

    private void CheckKnockBack()
    {
        if (Time.time >= _knockBackStartTime + _knockBackDuration && _knockBack)
        {
            _knockBack = false;
            _rb.linearVelocity = new Vector2(0.0f, _rb.linearVelocity.y);
        }
    }


    //this function is responsible for setting the dash velocity & checking if we should be dashing or we should stop
    /*private void CheckDash()
    {
        if (_dashTimeLeft > 0)
        {
            if (_isDashing)
            {
                _canMove = false;
                _canFlip = false;
                _rb.linearVelocity = new Vector2(dashSpeed * facingRightDirection, _rb.linearVelocity.y);
                _dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - _lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    _lastImageXpos = transform.position.x;
                }
            }

            if (_dashTimeLeft <= 0 || _isTouchingLedge)
            {
                _isDashing = false;
                _canMove = true;
                _canFlip = true;
            }
        }
    }
    */
    private void CheckWallSliding()
    {
        if (_isTouchingWall && _playerInputDirection == facingRightDirection && _rb.linearVelocity.y < 0 && !_canClimbLedge)
        {
            _isWallSliding = true;
        }
        else
        {
            _isWallSliding = false;
        }
    }
    public void FinishLedgeClimb()
    {
        _canClimbLedge = false;
        transform.position = _ledgePos2;
        _canMove = true;
        _canFlip = true;
        _ledgeDetected = false;
        _animator.SetBool("canClimbLedgeAnimatorParam", _canClimbLedge);
    }

    /*public void FinishLedgeClimb()
    {
        _canClimbLedge = false;
        transform.position = ledgePos2;
        _canMove = true;
        _canFlip = true;
        _ledgeDected = false;
        _animator.SetBool("canClimbLedgeAnimatorParam", _canClimbLedge);
    }
    */
    private void CheckSurroundings()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheckObjectTransform.position, groundCheckRadius, groundLayer);
        _isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, wallLayer);
        _isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, wallLayer);

        if (_isTouchingWall && !_isTouchingLedge && !_ledgeDetected)
        {
            _ledgeDetected = true;
            _ledgePosBot = wallCheck.position;
        }

        /*if (_isTouchingWall && !_isTouchingLedge && !_ledgeDected)
        {
            _ledgeDected = true;
            ledgePosBottom = wallCheck.position;
        }
        

        /*if (_rb.linearVelocity.x > 0.01f)
        {
            _isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, wallLayer);
        }
        else if (_rb.linearVelocity.x < -0.01f)
        {
            _isTouchingWall = Physics2D.Raycast(wallCheck.position, -transform.right, wallCheckDistance, wallLayer);
        }
        */
    }

    private void MovementDirection()
    {
        if (_isFacingRight && _playerInputDirection < 0)
        {
            FlipSprite();
        }
        else if (!_isFacingRight && _playerInputDirection > 0)
        {
            FlipSprite();
        }

        if (Mathf.Abs(_rb.linearVelocity.x) > 0.5f)
        {
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
        }
    }

    private void FlipSprite()
    {
        if (!_isWallSliding && _canFlip && !_knockBack)
        {
            facingRightDirection *= -1;
            _isFacingRight = !_isFacingRight;
            transform.Rotate(0.0f, 180f, 0.0f);
        }
    }
    private void CheckInput()
    {
        _playerInputDirection = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (_isGrounded || (_noOfJumpsLeft > 0 && _isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                _jumpBufferTimer = jumpBufferTime;
                _isAttemptingToJump = true;
            }
        }

        if (Input.GetButtonDown("Horizontal") && _isTouchingWall)
        {
            if (!_isGrounded && _playerInputDirection != facingRightDirection)
            {
                _canMove = false;
                _canFlip = false;
                _turnTimer = turnTimerSet;
            }
        }

        if (_turnTimer >= 0)
        {
            _turnTimer -= Time.deltaTime;
            if (_turnTimer <= 0)
            {
                _canMove = true;
                _canFlip = true;
            }
        }

        if (_checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            _checkJumpMultiplier = false;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * variableJumpForce);
        }

        if (Input.GetButtonDown("Dash"))
        {
            Debug.Log("Dash Pressed");
            if (Time.time >= (_lastDash + dashCoolDown))
            {
                AttempToDash();
            }
        }
    }

    private void AttempToDash()
    {
        _isDashing = true;
        _dashTimeLeft = dashTime;
        _lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        _lastImageXpos = transform.position.x; 
    }

    private void CheckDash()
    {
        if (_isDashing)
        {
            if (_dashTimeLeft > 0)
            {
                _canMove = false;
                _canFlip = false;
                _rb.linearVelocity = new Vector2(dashSpeed * facingRightDirection, _rb.linearVelocity.y);
                _dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - _lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    _lastImageXpos = transform.position.x;
                }
            }

            if (_dashTimeLeft <= 0 || _isTouchingWall)
            {
                _isDashing = false;
                _canMove = true;
                _canFlip = true;
            }
           
        }
    }

    /*private void AttempToDash()
    {
        _isDashing = true;
        _dashTimeLeft = dashTime;
        _lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        _lastImageXpos = transform.position.x;
    }
    */

    /*private void DoLedgeClimb()
    {
        if (_ledgeDected && !_canClimbLedge)
        {
            _canClimbLedge = true;

            if (_isFacingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBottom.x - wallCheckDistance) - XledgeClimbOffSet1, Mathf.Floor(ledgePosBottom.y) + YledgeClimbOffSet1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBottom.x - wallCheckDistance) - XledgeClimbOffSet2, Mathf.Floor(ledgePosBottom.y) + YledgeClimbOffSet1);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBottom.x - wallCheckDistance) - XledgeClimbOffSet1, Mathf.Ceil(ledgePosBottom.y) + YledgeClimbOffSet1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBottom.x - wallCheckDistance) - XledgeClimbOffSet2, Mathf.Ceil(ledgePosBottom.y) + YledgeClimbOffSet2);
            }
            _canMove = false;
            _canFlip = false;

            _animator.SetBool("canClimbLedgeAnimatorParam", _canClimbLedge);
        }

        if (_canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }
    */

    private void CheckLedgeClimb()
    {
        if (_ledgeDetected && !_canClimbLedge)
        {
            _canClimbLedge = true;

            if (_isFacingRight)
            {
                _ledgePos1 = new Vector2(Mathf.Floor(_ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(_ledgePosBot.y) + ledgeClimbXOffset1);
                _ledgePos2 = new Vector2(Mathf.Floor(_ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(_ledgePosBot.y) + ledgeClimbXOffset2);
            }
            else
            {
                _ledgePos1 = new Vector2(Mathf.Ceil(_ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset1, Mathf.Ceil(_ledgePosBot.y) + ledgeClimbXOffset1);
                _ledgePos2 = new Vector2(Mathf.Ceil(_ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Ceil(_ledgePosBot.y) + ledgeClimbXOffset2);
            }
            _canMove = false;
            _canFlip = false;

            _animator.SetBool("canClimbLedgeAnimatorParam", _canClimbLedge);
        }
        if (_canClimbLedge)
        {
            transform.position = _ledgePos1;
        }
    }
    private void CheckIfCanJumpTrue()
    {
        if (_isGrounded && _rb.linearVelocity.y <= 0.1f)
        {
            _noOfJumpsLeft = _noOfJumps;
        }
        if (_isTouchingWall)
        {
            _canWallJump = true;
        }

        if (_noOfJumpsLeft <= 0)
        {
            _canNormalJump = false;
        }
        else if (_noOfJumpsLeft > 0)
        {
            _canNormalJump = true;
        }
    }
    private void WhichJumpFunction()
    {
        if (_jumpBufferTimer > 0)
        {
            if (!_isGrounded && _isTouchingWall && _playerInputDirection != 0 && _playerInputDirection != facingRightDirection)
            {
                WallJump();
            }
            else if(_isGrounded)
            {
                NormalJump();
            }
        }
        if(_isAttemptingToJump)
        {
            _jumpBufferTimer -= Time.deltaTime;
        }

        if (_wallJumpTimer > 0)
        {
            if (_hasWallJump && _playerInputDirection == -lastWallJumpDirection)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0.0f);
                _hasWallJump = false;
            }
            else if (_wallJumpTimer <= 0)
            {
                _hasWallJump = false;
            }
            else
            {
                _wallJumpTimer -= Time.deltaTime;
            }
        }
    }
    private void NormalJump()
    {
        if (_canNormalJump)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, data.jumpForce);
            _noOfJumpsLeft--;
            _jumpBufferTimer = 0;
            _isAttemptingToJump = false;
            _checkJumpMultiplier = true;
        }
    }
    private void WallJump()
    {
        if (_canWallJump)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0.0f);
            _isWallSliding = false;
            _noOfJumpsLeft = _noOfJumps;
            _noOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * _playerInputDirection, wallJumpForce * wallJumpDirection.y);
            _rb.AddForce(forceToAdd,ForceMode2D.Impulse);
            _jumpBufferTimer = 0;
            _isAttemptingToJump = false;
            _checkJumpMultiplier = true;
            _turnTimer = 0;
            _canFlip = true;
            _canMove = true;
            _hasWallJump = true;
            _wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingRightDirection;
        }
    }
    private void ApplyMovement()
    {
        if (!_isGrounded && !_isWallSliding && _playerInputDirection == 0 && !_knockBack)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x * airDrag, _rb.linearVelocity.y);
        }
        else if(_canMove && !_knockBack)
        {
            _rb.linearVelocity = new Vector2(_playerInputDirection * data.horizontalSpeed, _rb.linearVelocity.y);
        }


        if (_isWallSliding)
        {
            if (_rb.linearVelocity.y < -wallSlideSpeed)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -wallSlideSpeed);
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckObjectTransform.position, groundCheckRadius);
        Gizmos.DrawRay(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));

    }

    public void EnableFlip()
    {
        _canFlip = true;
    }
    public void DisableFlip()
    {
        _canFlip = false;
    }

    //fun to return facing direction
    public int GetFacingDirection()
    {
        return facingRightDirection;
    }

}
