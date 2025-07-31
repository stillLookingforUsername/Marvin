using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("References")]
    public PlayerMovementStats MoveStats;
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;

    #region UI Input
    private bool moveLeft = false;
    private bool moveRight = false;
    private bool jumpPressed = false;

    #endregion

    private Animator _animator;
    private Rigidbody2D _rb;
    //jump audio
    public AudioClip audioClip;
    //movements vars
    public float HorizontalVelocity { get; private set; }
    private bool _isFacingRight;

    //collision check vars
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private RaycastHit2D _wallHit;
    private RaycastHit2D _lastWallHit;
    private bool _isGrounded;
    private bool _bumpedHead;
    private bool _isTouchingWall;

    //wall climb vars
    private bool _isWallClimbing;
    private float _wallClimbTimer;
    private float _wallClimbStaminaTimer;
    private bool _isWallClimbExhausted;
    private float _lastWallClimbTime;

    // wall grip vars
    private bool _isWallGripping;
    private float _wallGripTimer;
    [SerializeField] private float _wallGripDuration = 2f; // seconds


    //jump vars
    //public float VerticalVelocity { get; private set; }
    public float VerticalVelocity { get; set;}
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private float _numberOfJumpsUsed;

    //apex vars
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    //jump buffer vars
    private float _jumpBufferTimer;
    private bool _jumpReleaseDuringBuffer;

    //coyote time vars
    private float _coyoteTimer;


    //wall slide
    private bool _isWallSliding;
    private bool _isWallSlideFalling;

    //wall jump
    private bool _useWallJumpMoveStats;
    private bool _isWallJumping;
    private float _wallJumpTime;
    private bool _isWallJumpFastFalling;
    private bool _isWallJumpFalling;
    private float _wallJumpFastFallTime;
    private float _wallJumpFastFallReleaseSpeed;

    private float _wallJumpPostBufferTimer;

    private float _wallJumpApexPoint;
    private float _timePastWallJumpApexThreshold;
    private bool _isPastWallJumpApexThreshold;

    //dash vars
    private bool _isDashing;
    private bool _isAirDashing;
    private float _dashTimer;
    private float _dashOnGroundTimer;
    private int _numberOfDashesUsed;
    private Vector2 _dashDirection;
    private bool _isDashFastFalling;
    private float _dashFastFallTime;
    private float _dashFastFallReleaseSpeed;


    private void Awake()
    {
        _isFacingRight = true;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Wall grip input - support both keyboard (J key) and UI button
        if(_isTouchingWall && (Input.GetKeyDown(KeyCode.J) || InputManager.UIWallGripPressed) && !_isWallGripping)
        {
            StartWallGrip();
        }
        // Wall grip timer logic
        if (_isWallGripping)
        {
            _wallGripTimer -= Time.deltaTime;
            if (_wallGripTimer <= 0f || !_isTouchingWall)
            {
                StopWallGrip();
            }
        }
        JumpChecks();
        CountTimers();
        LandCheck();
        WallJumpCheck();
        DashCheck();
        WallClimbCheck();
        WallSlideCheck();
        UpdateAnimationStates();
    }

    #region Animation

    private void UpdateAnimationStates()
    {
        float yVel = VerticalVelocity;

        bool isMovingHorizontally = Mathf.Abs(HorizontalVelocity) > 0.1f;
        bool isRising = yVel > 0.1f;
        bool isFalling = yVel < -0.1f;

        // Wall climb takes priority over other states
        if (_isWallClimbing)
        {
            _animator.SetBool("isWallClimbing", true);
            _animator.SetBool("isWallSliding", false);
            _animator.SetBool("isRunning", false);
            _animator.SetBool("isJumping", false);
            _animator.SetBool("isFalling", false);
        }
        // Wall slide takes priority over other states
        else if (_isWallSliding || _isWallSlideFalling)
        {
            _animator.SetBool("isWallClimbing", false);
            _animator.SetBool("isWallSliding", true);
            _animator.SetBool("isRunning", false);
            _animator.SetBool("isJumping", false);
            _animator.SetBool("isFalling", false);
        }
        else
        {
            _animator.SetBool("isWallClimbing", false);
            _animator.SetBool("isWallSliding", false);
            _animator.SetBool("isRunning", isMovingHorizontally && _isGrounded);
            _animator.SetBool("isJumping", !_isGrounded && isRising);
            _animator.SetBool("isFalling", !_isGrounded && isFalling);
        }
        _animator.SetBool("isGrounded", _isGrounded);
    }


    #endregion

    private void FixedUpdate()
    {
        if (_isWallGripping)
        {
            HorizontalVelocity = 0f;
            VerticalVelocity = 0f;
            ApplyVelocity();
            return; // Skip other movement logic while gripping
        }
        CollisionChecks();
        Jump();
        Fall();
        WallClimb();
        WallSlide();
        WallJump();
        Dash();

        if (_isGrounded)
        {
            Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, InputManager.Movement);
        }
        else
        {
            //wall climb
            if (_isWallClimbing)
            {
                Move(MoveStats.WallClimbAcceleration, MoveStats.WallClimbDeceleration, InputManager.Movement);
            }
            //wall jump
            else if (_useWallJumpMoveStats)
            {
                Move(MoveStats.WallJumpMoveAcceleration, MoveStats.WallJumpMoveDeceleration, InputManager.Movement);
            }
            //airborne
            else
            {
                Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, InputManager.Movement);
            }
        }
        ApplyVelocity();
    }
    private void ApplyVelocity()
    {
        //Clamp Fall Speed
        if (!_isDashing)
        {
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);
        }

        else
        {
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -50f, 50f);
        }

            _rb.linearVelocity = new Vector2(HorizontalVelocity, VerticalVelocity);
    }

    #region Movement
    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (!_isDashing)
        {

        if (Mathf.Abs(moveInput.x) >= MoveStats.MoveThreshold)
        {
            //check if he needs to turn
            TurnCheck(moveInput);

            float targetVelocity = 0f;
            if (InputManager.RunIsHeld)
            {
                targetVelocity = moveInput.x * MoveStats.MaxRunSpeed;
            }
            else
            {
                targetVelocity = moveInput.x * MoveStats.MaxRunSpeed;
            }
            HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else if (Mathf.Abs(moveInput.x) < MoveStats.MoveThreshold)
        {
            HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, 0f, deceleration * Time.fixedDeltaTime);
        }
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (_isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!_isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }
    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            _isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            _isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }
    #endregion

    #region Land/Fall


    private void LandCheck()
    {
        //landed
        if ((_isJumping || _isFalling || _isWallJumpFalling || _isWallJumping || _isWallSlideFalling || _isWallSliding || _isDashFastFalling) && _isGrounded && VerticalVelocity <= 0f)
        {
            ResetJumpValues();
            StopWallSlide();
            ResetWallJumpValues();
            ResetDashes();

            _numberOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;

            if (_isDashFastFalling && _isGrounded)
            {
                ResetDashValues();
                return;
            }
            ResetDashValues();
        }
    }

    private void Fall()
    {
        //Normal Gravity While Falling
        if (!_isGrounded && !_isJumping && !_isWallSliding && !_isWallJumping && !_isDashing && !_isDashFastFalling)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }
            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }
    }
    #endregion

    #region Jump

    private void ResetJumpValues()
    {
        _isJumping = false;
        _isFalling = false;
        _isFastFalling = false;
        _fastFallTime = 0f;
        _isPastApexThreshold = false;
    }
    private void JumpChecks()
    {
        //When we press the Jump Button
        if (InputManager.JumpWasPressed)
        {
            if (_isWallSlideFalling && _wallJumpPostBufferTimer >= 0f)
            {
                return;
            }
            else if (_isWallSliding || (_isTouchingWall && _isGrounded))
            {
                return;
            }

                _jumpBufferTimer = MoveStats.JumpBufferTime;
            _jumpReleaseDuringBuffer = false;
        }

        //when we release the Jump Button
        if (InputManager.JumpWasReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleaseDuringBuffer = true;
            }
            if (_isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = MoveStats.TimeForUpwardCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        //Initiate Jump With Jump Buffering and Coyote Time
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (_jumpReleaseDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        //double Jump
        else if (_jumpBufferTimer > 0f && (_isJumping || _isWallJumping || _isWallSlideFalling || _isAirDashing || _isDashFastFalling) && !_isTouchingWall && _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
        {
            _isFastFalling = false;
            InitiateJump(1);

            if (_isDashFastFalling)
            {
                _isDashFastFalling = false;
            }
        }

        //Air Jump after Coyote time is Lapsed
        else if (_jumpBufferTimer > 0f && _isFalling && !_isWallSlideFalling &&  _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(2);
            _isFastFalling = false;
        }



    }
    private void InitiateJump(int numbersOfJumpsUsed)
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }

        ResetWallJumpValues();

        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += numbersOfJumpsUsed;
        VerticalVelocity = MoveStats.InitialJumpVelocity;
        SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform, 1f);
    }
    private void Jump()
    {
        //Apply Gravity while Jumping
        if (_isJumping)
        {
            //check for head bump
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }
            //Gravity on Ascending
            if (VerticalVelocity >= 0f)
            {
                //Apex Control
                _apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (_apexPoint > MoveStats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                //Gravity on Ascending but not past apex Threshold
                else if(!_isFastFalling)
                {
                    VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }
            }
            //Gravity on Descending
            else if (!_isFastFalling)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (VerticalVelocity < 0f)
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                }
            }
        }

        //Jump Cut

        if (_isFastFalling)
        {
            if (_fastFallTime >= MoveStats.TimeForUpwardCancel)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_fastFallTime < MoveStats.TimeForUpwardCancel)
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / MoveStats.TimeForUpwardCancel));
            }
            _fastFallTime += Time.fixedDeltaTime;
        }

    

    }

    #endregion


    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MoveStats.HeadDetectionRayLength, MoveStats.GroundLayer);
        if (_headHit.collider != null)
        {
            _bumpedHead = true;
        }
        else
        {
            _bumpedHead = false;
        }

        //Debug Visualization

        if (MoveStats.DebugShowHeadBumpBox)
        {
            float headWidth = MoveStats.HeadWidth;

            Color rayColor;
            if (_bumpedHead)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector2.up * MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + MoveStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
        }


    }

    private void IsTouchingWall()
    {
        float originEndPoint = 0f;
        if (_isFacingRight)
        {
            originEndPoint = _bodyColl.bounds.max.x;
        }
        else
        {
            originEndPoint = _bodyColl.bounds.min.x;
        }
        float adjustedHeight = _bodyColl.bounds.size.y * MoveStats.WallDetectionRayHeightMultiplier;

        Vector2 boxCastOrigin = new Vector2(originEndPoint, _bodyColl.bounds.center.y);
        Vector2 boxCastSize = new Vector2(MoveStats.WallDetectionRayLength, adjustedHeight);

        _wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, MoveStats.WallDetectionRayLength, MoveStats.GroundLayer);

        if (_wallHit.collider != null)
        {
            _lastWallHit = _wallHit;
            _isTouchingWall = true;
        }
        else
        {
            _isTouchingWall = false;
        }

        #region Debug Visualization

        if (MoveStats.DebugShowWallHitBox)
        {
            Color rayColor;
            if (_isTouchingWall)
            {
                rayColor = Color.green;
            }
            else { rayColor = Color.red; }

            Vector2 boxBottomLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
            Vector2 boxBottomRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
            Vector2 boxTopLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);
            Vector2 boxTopRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);

            Debug.DrawLine(boxBottomLeft,boxBottomRight, rayColor);
            Debug.DrawLine(boxBottomRight,boxTopRight, rayColor);
            Debug.DrawLine(boxTopRight, boxTopLeft, rayColor);
            Debug.DrawLine(boxTopLeft,boxBottomLeft,rayColor);
        }


        #endregion
    }

    #region Wall Jump

    private void WallJumpCheck()
    {
        if (ShouldApplyPostWallJumpBuffer())
        {
            _wallJumpPostBufferTimer = MoveStats.WallJumpPostBufferTime;
        }
        //wall jump fast falling
        if (InputManager.JumpWasReleased && !_isWallSliding && !_isTouchingWall && _isWallJumping)
        {
            if (VerticalVelocity > 0f)
            {
                if (_isPastWallJumpApexThreshold)
                {
                    _isPastWallJumpApexThreshold = false;
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallTime = MoveStats.TimeForUpwardCancel;

                    VerticalVelocity = 0f;
                }
                else
                {
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }
        //Actual jump with post wall jump buffer time
        if (InputManager.JumpWasPressed && _wallJumpPostBufferTimer > 0f)
        {
            InitiateWallJump();
        }
    }

    private void InitiateWallJump()
    {
        if (!_isWallJumping)
        {
            _isWallJumping = true;
            _useWallJumpMoveStats = true;
        }
        StopWallSlide();
        ResetJumpValues();
        _wallJumpTime = 0f;

        VerticalVelocity = MoveStats.InitialWallJumpVelocity;

        int dirMultiplier = 0;
        Vector2 hitPoint = _lastWallHit.collider.ClosestPoint(_bodyColl.bounds.center);

        if (hitPoint.x > transform.position.x)
        {
            dirMultiplier = -1;
        }
        else
        {
            dirMultiplier = 1;
        }
        HorizontalVelocity = Mathf.Abs(MoveStats.WallJumpDirection.x) * dirMultiplier;
    }
    private void WallJump()
    {
        //apply Wall jump Gravity
        if (_isWallJumping)
        {
            //Time to take over Movement Controls while wall jumping
            _wallJumpTime += Time.fixedDeltaTime;
            if (_wallJumpTime >= MoveStats.TimeTillJumpApex)
            {
                _useWallJumpMoveStats = false;
            }

            //Hit head
            if (_bumpedHead)
            {
                _isWallJumpFastFalling = true;
                _useWallJumpMoveStats = false;
            }
            //Gravity in Acending
            if (VerticalVelocity >= 0f)
            {
                //Apex Controls
                _wallJumpApexPoint = Mathf.InverseLerp(MoveStats.WallJumpDirection.y, 0f, VerticalVelocity);

                if (_wallJumpApexPoint > MoveStats.ApexThreshold)
                {
                    if (!_isPastWallJumpApexThreshold)
                    {
                        _isPastWallJumpApexThreshold = true;
                        _timePastWallJumpApexThreshold = 0f;
                    }
                    if (_isPastWallJumpApexThreshold)
                    {
                        _timePastWallJumpApexThreshold += Time.fixedDeltaTime;
                        if (_timePastWallJumpApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                //Gravity In Ascending But not Past Apex Threshold
                else if (!_isWallJumpFastFalling)
                {
                    VerticalVelocity += MoveStats.WallJumpGravity * Time.fixedDeltaTime;
                    if (_isPastWallJumpApexThreshold)
                    {
                        _isPastWallJumpApexThreshold = false;
                    }
                }

            }
            //Gravity on Descending
            else if (!_isWallJumpFastFalling)
            {
                VerticalVelocity += MoveStats.WallJumpGravity * Time.fixedDeltaTime;
            }
            else if (VerticalVelocity < 0f)
            {
                if (!_isWallJumpFalling)
                {
                    _isWallJumpFalling = true;
                }
            }
        }
        //Handle Wall Jump Cut Time
        if (_isWallJumpFastFalling)
        {
            if (_wallJumpFastFallTime >= MoveStats.TimeForUpwardCancel)
            {
                VerticalVelocity += MoveStats.WallJumpGravity * MoveStats.WallJumpGravityOnReleaseMultiplier * Time.deltaTime;
            }
            else if (_wallJumpFastFallTime < MoveStats.TimeForUpwardCancel)
            {
                VerticalVelocity = Mathf.Lerp(_wallJumpFastFallReleaseSpeed, 0f, (_wallJumpFastFallTime / MoveStats.TimeForUpwardCancel));
            }
            _wallJumpFastFallTime += Time.fixedDeltaTime;
        }
    }

    private bool ShouldApplyPostWallJumpBuffer()
    {
        if (!_isGrounded && (_isTouchingWall || _isWallSliding))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void ResetWallJumpValues()
    {
        _isWallSlideFalling = false;
        _useWallJumpMoveStats = false;
        _isWallJumping = false;
        _isWallJumpFastFalling = false;
        _isWallJumpFalling = false;
        _isPastWallJumpApexThreshold = false;

        _wallJumpFastFallTime = 0f;
        _wallJumpTime = 0f;
    }
    #endregion

    #region Dash

    private void DashCheck()
    {
        if (InputManager.DashWasPressed)
        {
            //ground dash
            if (_isGrounded && _dashOnGroundTimer < 0 && !_isDashing)
            {
                InitiateDash();
            }

            //air dash
            else if (!_isGrounded && !_isDashing && _numberOfDashesUsed < MoveStats.NumberOfDashes)
            {
                _isAirDashing = true;
                InitiateDash();

                //you left a wallSlide but dashed within  the wall jump post buffer time
                if (_wallJumpPostBufferTimer > 0f)
                {
                    _numberOfJumpsUsed--;
                    if (_numberOfJumpsUsed < 0f)
                    {
                        _numberOfJumpsUsed = 0;
                    }
                }
            }
        }
    }
    private void InitiateDash()
    {
        _dashDirection = InputManager.Movement;

        Vector2 closestDirection = Vector2.zero;
        float minDistance = Vector2.Distance(_dashDirection, MoveStats.DashDirections[0]);
        for (int i = 0; i < MoveStats.DashDirections.Length; i++)
        {
            //skip it if we  hit it bang on
            if (_dashDirection == MoveStats.DashDirections[i])
            {
                closestDirection = _dashDirection;
                break;
            }
            float distance = Vector2.Distance(_dashDirection, MoveStats.DashDirections[i]);

            //check if this is a diagonal direction and apply bias
            bool isDiagonal = (Mathf.Abs(MoveStats.DashDirections[i].x) == 1 && Mathf.Abs(MoveStats.DashDirections[i].y) == 1);
            if (isDiagonal)
            {
                distance -= MoveStats.DashDiagonallyBias;
            }
            else if (distance < minDistance)
            {
                minDistance = distance;
                closestDirection = MoveStats.DashDirections[i];
            }
        }
        //handle direction with no input
        if (closestDirection == Vector2.zero)
        {
            if (_isFacingRight)
            {
                closestDirection = Vector2.right;
            }
            else
            {
                closestDirection = Vector2.left;
            }
        }
        _dashDirection = closestDirection;
        _numberOfDashesUsed++;
        _isDashing = true;
        _dashTimer = 0f;
        _dashOnGroundTimer = MoveStats.TimeBtwDashesOnGround;

        ResetJumpValues();
        ResetWallJumpValues();
        StopWallSlide();
    }
    private void Dash()
    {
        if (_isDashing)
        {
            //stop the dash after the timer
            _dashTimer += Time.fixedDeltaTime;
            if (_dashTimer >= MoveStats.DashTime)
            {
                if (_isGrounded)
                {
                    ResetDashes();
                }
                _isAirDashing = false;
                _isDashing = false;

                if (!_isJumping && !_isWallJumping)
                {
                    _dashFastFallTime = 0f;
                    _dashFastFallReleaseSpeed = VerticalVelocity;

                    if (!_isGrounded)
                    {
                        _isDashFastFalling = true;
                    }
                }
                return;
            }
            HorizontalVelocity = MoveStats.DashSpeed * _dashDirection.x;

            if (_dashDirection.y != 0f || _isAirDashing)
            {
                VerticalVelocity = MoveStats.DashSpeed * _dashDirection.y;
            }
        }
        //Handle Dash cut
        else if (_isDashFastFalling)
        {
            if (VerticalVelocity > 0f)
            {
                if (_dashFastFallTime < MoveStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity = Mathf.Lerp(_dashFastFallReleaseSpeed, 0f, (_dashFastFallTime / MoveStats.DashTimeForUpwardsCancel));
                }
                else if (_dashFastFallTime >= MoveStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity += MoveStats.Gravity * MoveStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }
                _dashFastFallTime += Time.fixedDeltaTime;
            }
            else
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
        }
    }

    private void ResetDashValues()
    {
        _isDashFastFalling = false;
        _dashOnGroundTimer = -0.01f;
    }

    private void ResetDashes()
    {
        _numberOfDashesUsed = 0;
    }
    #endregion

    #region Collision Checks

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, MoveStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionRayLength, MoveStats.GroundLayer);

        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
        #region Debug Visualization

        if (MoveStats.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (_isGrounded)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - MoveStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }

        #endregion
    }
    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
        IsTouchingWall();
    }

    #endregion

    #region Draw Jump Arc

    private void DrawJumpArc(float moveSpeed, Color gizmosColor)
    {
        Vector2 startPosition = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 previousPosition = startPosition;
        float speed = 0f;

        if (MoveStats.DrawRight)
        {
            speed = moveSpeed;
        }
        else
        {
            speed = -moveSpeed;
        }
        Vector2 velocity = new Vector2(speed, MoveStats.InitialJumpVelocity);
        Gizmos.color = gizmosColor;

        float timeStep = 2 * MoveStats.TimeTillJumpApex / MoveStats.ArcResolution;  //time Step for the Simulation
        //float totalTime = (2 * MoveStats.TimeTillJumpApex) + MoveStatx.ApexHangTime;

        for (int i = 0; i < MoveStats.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if (simulationTime < MoveStats.TimeTillJumpApex)    //Ascending
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2(0, MoveStats.Gravity) * simulationTime * simulationTime;
            }
            else if (simulationTime < MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime)  //Apex hang time
            {
                float apexTime = simulationTime - MoveStats.TimeTillJumpApex;
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime;   //no vertical movement during hang time
            }
            else //Descending
            {
                float descendTime = simulationTime - (MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime);
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * MoveStats.ApexHangTime;
                displacement += new Vector2(speed, 0) * descendTime + 0.5f * new Vector2(0, MoveStats.Gravity) * descendTime * descendTime;
            }

            drawPoint = startPosition + displacement;

            if (MoveStats.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), MoveStats.GroundLayer);
                if (hit.collider != null)
                {
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }
            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }

    #endregion

    #region Timers

    #region wall slide

    private void WallSlideCheck()
    {
        if (_isTouchingWall && !_isGrounded && !_isDashing && !_isWallClimbing)
        {
            if (VerticalVelocity < 0f && !_isWallSliding)
            {
                ResetJumpValues();
                ResetWallJumpValues();
                ResetDashValues();

                if (MoveStats.ResetDashOnWallSlide)
                {
                    ResetDashes();
                }

                _isWallSlideFalling = false;
                _isWallSliding = true;
                if (MoveStats.ResetJumpsOnWallSlide)
                {
                    _numberOfJumpsUsed = 0;
                }
            }
        }
        else if (_isWallSliding && (!_isTouchingWall || _isGrounded || _isWallSlideFalling || _isWallClimbing))
        {
            StopWallSlide();
        }
    }

    private void StopWallSlide()
    {
        if (_isWallSliding)
        {
            _numberOfJumpsUsed++;

            _isWallSliding = false;
        }
    }
    private void WallSlide()
    {
        if (_isWallSliding)
        {
            VerticalVelocity = Mathf.Lerp(VerticalVelocity, -MoveStats.WallSlideSpeed, MoveStats.WallSlideDecelerationSpeed * Time.fixedDeltaTime);
        }
    }

    #endregion

    private void CountTimers()
    {
        //jump buffer
        _jumpBufferTimer -= Time.deltaTime;

        //jumo coyote time
        if (!_isGrounded)
        {
            _coyoteTimer -= Time.deltaTime;
        }
        else
        {
            _coyoteTimer = MoveStats.JumpCoyoteTime;
        }
        //wall jump buffer time
        if (!ShouldApplyPostWallJumpBuffer())
        {
            _wallJumpPostBufferTimer -= Time.deltaTime;
        }
        //dash timer
        if (_isGrounded)
        {
            _dashOnGroundTimer -= Time.deltaTime;
        }
    }

    private void WallClimbCheck()
    {
        // Start wall climbing when touching wall and pressing up
        if (_isTouchingWall && !_isGrounded && !_isDashing && !_isWallJumping)
        {
            if (InputManager.Movement.y > 0.1f && !_isWallClimbExhausted)
            {
                if (!_isWallClimbing)
                {
                    _isWallClimbing = true;
                    _wallClimbTimer = 0f;
                    StopWallSlide();
                    ResetJumpValues();
                    ResetWallJumpValues();
                }
            }
            // Allow sliding down while wall climbing
            else if (_isWallClimbing && InputManager.Movement.y < -0.1f)
            {
                StopWallClimb();
                return;
            }
        }

        // Stop wall climbing if conditions are not met
        if (_isWallClimbing && (!_isTouchingWall || _isGrounded || _isDashing || _isWallJumping))
        {
            StopWallClimb();
        }

        // Handle wall climb stamina
        if (_isWallClimbing)
        {
            _wallClimbTimer += Time.deltaTime;
            if (_wallClimbTimer >= MoveStats.MaxWallClimbTime)
            {
                _isWallClimbExhausted = true;
                _lastWallClimbTime = Time.time;
                StopWallClimb();
            }
        }
        else if (_isGrounded && _isWallClimbExhausted)
        {
            if (Time.time - _lastWallClimbTime >= MoveStats.WallClimbStaminaRecoveryTime)
            {
                _isWallClimbExhausted = false;
                _wallClimbTimer = 0f;
                _wallClimbStaminaTimer = 0f;
            }
        }
    }

    private void WallClimb()
    {
        if (_isWallClimbing)
        {
            // Apply vertical movement with smooth acceleration
            float targetVelocity = MoveStats.WallClimbSpeed * InputManager.Movement.y;
            VerticalVelocity = Mathf.Lerp(VerticalVelocity, targetVelocity, MoveStats.WallClimbAcceleration * Time.fixedDeltaTime);
            
            // Keep player close to wall with consistent force
            float horizontalForce = _isFacingRight ? MoveStats.WallClimbHorizontalForce : -MoveStats.WallClimbHorizontalForce;
            HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, horizontalForce, MoveStats.WallClimbAcceleration * Time.fixedDeltaTime);
        }
    }

    private void StopWallClimb()
    {
        if (_isWallClimbing)
        {
            _isWallClimbing = false;
            // Apply a small vertical velocity to prevent instant falling
            VerticalVelocity = Mathf.Max(VerticalVelocity, -2f);
        }
    }

    #endregion

    private void StartWallGrip()
    {
        _isWallGripping = true;
        _wallGripTimer = _wallGripDuration;
        HorizontalVelocity = 0f;
        VerticalVelocity = 0f;
    }

    private void StopWallGrip()
    {
        _isWallGripping = false;
    }

    private void OnDrawGizmos()
    {
        if (MoveStats.ShowWalkJumpArc)
        {
            DrawJumpArc(MoveStats.MaxWalkSpeed,Color.white);
        }
        if (MoveStats.ShowRunJumpArc)
        {
            DrawJumpArc(MoveStats.MaxRunSpeed, Color.red);
        }

    }
}





