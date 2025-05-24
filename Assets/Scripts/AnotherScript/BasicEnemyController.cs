using UnityEngine;

public class BasicEnemyController : MonoBehaviour 
{
    //flip error fix
    private float _lastFlipTime;
    [SerializeField] private float _flipCooldown = 0.2f;

    //we will use state machine here,so that we can extend the enemy behaviour later on if we want to 
    private enum State 
    {
        //list of states
        Moving,
        KnockBack,
        Dead
    }

    private State _currentState;    //to keep track of what our current state is
    private int 
        _facingDirection,
        _damageDirection;
    private Vector2
        _movement,  /*here we will update the values of _movement var to apply to the rb.linearVelocity instead of declaring
                     * a new Vector2 everytime we update it
                     */
        _touchDamageBotLeft, /*to detect the player we will use physics 2d overlap area,overlap area takes 2 parameters
                              * the bottom_left_corner & top_right_corner
                              */
        _touchDamageTopRight;

    [SerializeField]
    private float
        _groundCheckDistance,
        _wallCheckDistance,
        _movementSpeed,
        _maxHealth,
        _knockBackDuration,
        _touchDamageCoolDown,       
        _touchDamage,   /*the way the will will work is there will be a bounding box/little area over the
                         *enemy where if the player enter that area he will take damage , and we need to set a width & height
                         *for that that why we r using _touchDamgeWidth & _touchDamgeHeight
                         */
        _touchDamageWidth,
        _touchDamageHeight;

    private float _lastTouchDamageTime;   //last the our enemy damaged our player

    [SerializeField]
    private Transform
        _groundCheck,
        _wallCheck,
        _touchDamageCheck;  //this will be a gameObject that a child of our enemey that will be the center of our touchDamage area

    [SerializeField]
    private GameObject
        _hitParticle,
        _deathChunkParticle,
        _deathBloodParticle;
    [SerializeField]
    private Vector2 knockBackSpeed;  //will allow us to specify x&y speeds in one variable
    [SerializeField]
    private LayerMask 
        ground,
        wall,
        whatisPlayer;

    private GameObject _alive;     //ref for the gameObject that holds the sprite
    private Rigidbody2D _rbAlive;
    private Animator _animAlive;

    private bool
        _groundDetected,
        _wallDetected;
    private float 
        _currentHealth,
        _knockBackStartTime;

    private float[] _attackDetails = new float[2];  /*to keep track of the attack details that we r going to send to our player via
                                                     * the SendMessage() function
                                                     */

    private void Start()
    {
        _alive = transform.Find("Alive").gameObject;
        _rbAlive = _alive.GetComponent<Rigidbody2D>();
        _facingDirection = 1;
        _animAlive = _alive.GetComponent<Animator>();
        _currentHealth = _maxHealth;
        //SwitchState(State.Moving);

    }
    private void Update()
    {
        //here we will determine which state is current active & based on that we will call the diff update func for the different State
        switch (_currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.KnockBack:
                UpdateKnockBackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }

    }



    //--Moving State-------------------------------------------------------------

    private void EnterMovingState()
    {

    }
    private void UpdateMovingState()
    {
        _groundDetected = Physics2D.Raycast(_groundCheck.position, Vector2.down, _groundCheckDistance, ground);
        //_wallDetected = Physics2D.Raycast(_wallCheck.position, Vector2.right, _wallCheckDistance, wall);
        _wallDetected = Physics2D.Raycast(_wallCheck.position, Vector2.right * _facingDirection, _wallCheckDistance, wall);

        CheckTouchDamage();

        //if grounDetecte is false or wallDetected is true then Flip()
        if (!_groundDetected || _wallDetected)
        {
            //Flip the enemy
            /*if (_wallDetected)
            {
                Debug.Log("Wall Detected");
            }
            */
                Flip();
        }
        else
        {
            //Move the enemy
            _movement.Set(_movementSpeed * _facingDirection, _rbAlive.linearVelocity.y);
            _rbAlive.linearVelocity = _movement;
        }
    }
    private void ExitMovingState()
    {

    }

    //--KnockBack State--------------------------------------------------------------

    private void EnterKnockBackState()
    {
        _knockBackStartTime = Time.time;
        _movement.Set(knockBackSpeed.x * _damageDirection, knockBackSpeed.y);
        _rbAlive.linearVelocity = _movement;

        //here we also need to take care of transitioning in the animator
        _animAlive.SetBool("knockBackAnimatorParam", true);
    }
    private void UpdateKnockBackState()
    {
        if (Time.time > _knockBackStartTime + _knockBackDuration)
        {
            SwitchState(State.Moving);
        }
    }
    private void ExitKnockBackState()
    {
        _animAlive.SetBool("knockBackAnimatorParam", false);
    }

    //--Dead State-----------------------------------------------------------------

    private void EnterDeadState()
    {
        //spawn chunks & blood
        Instantiate(_deathChunkParticle, _alive.transform.position, _deathChunkParticle.transform.rotation);
        Instantiate(_deathBloodParticle, _alive.transform.position, _deathBloodParticle.transform.rotation);
        Destroy(gameObject);
    }
    private void UpdateDeadState()
    {

    }
    private void ExitDeadState()
    {

    }

    //--Other functions------------------------------------------------------------

    //diff between the Damage(float var) in combatDummyController and this is that this one take array as arguments(param)
    /*the reason we use array here is cuz we use the .SendMessage() function when we hit sth,the sendMessage function only
     * allow us to send one param in,so if we want to send multiple parameters through then we have to do like this,
     * & the reason we need multiple parameters is cuz we'r going to be sending through the attackDamgage & also going to 
     * send through x location of the person doing the attack that way we know which side of us the enemy is standing on & which 
     * way we need to get knockback
     */
    private void Damage(float[] attackDetails)
    {
        _currentHealth -= attackDetails[0];
        Instantiate(_hitParticle, _alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        /*x pos of our player is greater than x pos of our enemey meaning d player is on the right side of the enemy
         * so the the enemy should be knockback to the left direction meaning damageDirectoin = -1,
         * else if player standing on left and player.position < enmey.position, meaning the damage will be from left side
         * meaning the damageDirection = 1 (enemy will be knockBack in the right directon since the player hit the enemy frm left side
        */
        if (attackDetails[1] > _alive.transform.position.x)
        {
            _damageDirection = -1;
        }
        else
        {
            _damageDirection = 1;
        }

        //Hit particle
        if (_currentHealth > 0.0f)
        {
            //enemy still alive
            SwitchState(State.KnockBack);
        }
        else if (_currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }

    private void CheckTouchDamage()
    {
        if (Time.time >= _lastTouchDamageTime + _touchDamageCoolDown)
        {
            _touchDamageBotLeft.Set(_touchDamageCheck.position.x - (_touchDamageWidth / 2), _touchDamageCheck.position.y - (_touchDamageHeight / 2));
            _touchDamageTopRight.Set(_touchDamageCheck.position.x + (_touchDamageWidth / 2), _touchDamageCheck.position.y + (_touchDamageHeight / 2));

            Collider2D hit = Physics2D.OverlapArea(_touchDamageBotLeft, _touchDamageTopRight, whatisPlayer);

            if (hit != null)
            {
                _lastTouchDamageTime = Time.time;
                _attackDetails[0] = _touchDamage;
                _attackDetails[1] = _alive.transform.position.x;  /*we r passing the x pos of the enemy to the player that way the player
                                                                   *knows frm what side he's being damaged meaning
                                                                   *we can apply the knockBack in the right direction
                                                                   */
                hit.SendMessage("Damage", _attackDetails);
            }
        }
    }

    private void Flip()
    {
        if (Time.time - _lastFlipTime < _flipCooldown) return; //changes made

        _facingDirection *= -1;
        _alive.transform.Rotate(0.0f, 180.0f, 0.0f);

        _lastFlipTime = Time.time;  //changes made

        transform.position += new Vector3(_facingDirection * 0.1f, 0.0f, 0.0f);
    }


    //a function that will take care of swaping to into diff states
    private void SwitchState(State state)       //param(State state) the state we wanna swap to
    {
        //1st switch will take care of calling the ExitFunction for the currentState
        switch (_currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.KnockBack:
                ExitKnockBackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        //2nd switch will take care of calling the EnterFunction for the State we wanna go into
        switch (state)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.KnockBack:
                EnterKnockBackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }
        _currentState = state;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_groundCheck.position, new Vector2(_groundCheck.position.x, _groundCheck.position.y - _groundCheckDistance));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_wallCheck.position, new Vector2(_wallCheck.position.x + _wallCheckDistance, _wallCheck.position.y));

        /*we r gonna draw the box that damages the player for that we need to draw lines connecting the 4 corners of the box,
         *so first we need to declare 4 vector2 for each one of the corners & then determine what those are
         * and finally  we need to draw those lines
         */
        Vector2 botLeft = new Vector2(_touchDamageCheck.position.x - (_touchDamageWidth / 2), _touchDamageCheck.position.y - (_touchDamageHeight / 2));
        Vector2 botRight = new Vector2(_touchDamageCheck.position.x + (_touchDamageWidth / 2), _touchDamageCheck.position.y - (_touchDamageHeight / 2));
        Vector2 topLeft = new Vector2(_touchDamageCheck.position.x - (_touchDamageWidth / 2), _touchDamageCheck.position.y + (_touchDamageHeight / 2));
        Vector2 topRight = new Vector2(_touchDamageCheck.position.x + (_touchDamageWidth / 2), _touchDamageCheck.position.y + (_touchDamageHeight / 2));

        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(botLeft, topLeft);
        Gizmos.DrawLine(botRight, topRight);
    }

}
