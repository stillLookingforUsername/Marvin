using System.IO;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private bool _combatEnable;
    [SerializeField] private float _inputTimer;     //how long the input will be hold (eg : if u press now it will work till 3 sec starting frm now)
    [SerializeField] private float _attack1Radius;  //radius for the physics2d.Circle
    [SerializeField] private float _attack1Damage;  //to store the amount of damage player can do
    [SerializeField] private Transform _attack1HitBoxPos;   /*this will store a ref to a gameObject we will create as a child to our player
                                                             *& will allow us to position our hitBox where we want
                                                            */
    [SerializeField] private LayerMask whatIsDamageable;    //will use to make it, so that we know what object is actually needed to be detected and which don't

    private bool _gotInput, _isAttacking, _isFirstAttack;

    //set to NegativeInfinity so that we r always ready to attack frm the start of the game
    private float _lastInputTime = Mathf.NegativeInfinity;   //responsible for storing the last time we attempted to attack (eg: 3 sec ago i attempted to attack)

    private Animator _animator;
    private float[] _attackDetails = new float[2];

    private PlayerController _playerController;
    private PlayerState _playerState;

    private void Start()
    {
        _combatEnable = true;
        _animator = GetComponent<Animator>();
        _animator.SetBool("canAttackAnimatorParam", _combatEnable);
        _playerController = GetComponent<PlayerController>();
        _playerState = GetComponent<PlayerState>();
    }


    private void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }


    //function to check input
    private void CheckCombatInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left Click");
            if (_combatEnable)
            {
                //Attemp combat
                _gotInput = true;
                _lastInputTime = Time.time;     //Record the current time(in seconds since the game started)
            }
        }
    }

    //making the attack happen when we get an input
    private void CheckAttacks()
    {
        if (_gotInput)
        {
            //perform attack 1
            if (!_isAttacking)    //if we are not currently in attack animation
            {
                _gotInput = false;
                _isAttacking = true;
                _isFirstAttack = !_isFirstAttack;
                _animator.SetBool("attack1AnimatorParam", true);
                _animator.SetBool("firstAttackAnimatorParam", _isFirstAttack);
                _animator.SetBool("isAttackingAnimatorParam", _isAttacking);
            }
        }
        if (Time.time >= _lastInputTime + _inputTimer)       // suppose now it 6 sec & lastInputTime = 3sec and inputTimme = 3 , so we can attack
        {
            //wait for new input
            _gotInput = false;
        }
    }

    //this fun will be called when we get to the Impact_Part of the animation & will detect all damagable objects in a range and damage them
    private void CheckAttackHitBox()
    {
        Collider2D[] dectedObjects = Physics2D.OverlapCircleAll(_attack1HitBoxPos.position,_attack1Radius,whatIsDamageable);

        _attackDetails[0] = _attack1Damage;
        _attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in dectedObjects)
        {
            Debug.Log("Detected Obj : " + collider.gameObject.name);
            /*if (collider.gameObject.name == "Alive")
            {
                collider.transform.parent.SendMessage("Damage", _attackDetails);
            }
            else
            {
                collider.transform.parent.SendMessage("Damage", _attackDetails[0]);
            }
            */
            
            //collider.transform.parent.SendMessage("Damage", _attackDetails);
            //u can put this 2 lines it will work but still there will be error in console
            collider.transform.parent.SendMessage("Damage", _attackDetails);
            //collider.transform.parent.SendMessage("Damage", _attackDetails[0]);
            //Instantiate hit particles
        }
    }

    //will be called at the end of the attack Animation, & let our script know that it's done
    private void FinishAttack1()
    {
        _isAttacking = false;
        _animator.SetBool("isAttackingAnimatorParam", _isAttacking);
        _animator.SetBool("attack1AnimatorParam", false);
    }

    private void Damage(float[] attackDetails)
    {
        if (!_playerController.GetDashCheck())
        {
            int direction;  //what direction the player is suppose to get knockBacked

            //implement damage to player using attackDetails[0] here -----------
            _playerState.DecreaseHealth(attackDetails[0]);

            if (attackDetails[1] < transform.position.x)    // attackDetails[1](pos of enemy) is < x pos of player, means it's on left side of player and also the enemy is facing right
            {
                direction = 1;  //player gets knockback at right side
            }
            else
            {
                direction = -1; //player gets knockback at left side
            }
            _playerController.KnockBack(direction);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_attack1HitBoxPos.position, _attack1Radius);
    }
}
