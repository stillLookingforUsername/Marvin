using UnityEngine;

public class CombatDummyController : MonoBehaviour {

    [SerializeField] private float _maxHealth;  //store maximum health of dummy
    [SerializeField] private float _knockBackSpeedX, _knockBackSpeedY, _knockBackDuration;  //x&y velocity when we apply knockBack
    [SerializeField] private float _knockBackDeathSpeedX, _knockBackDeathSpeedY, _deathTorque; /*x & y use to knock the dummy further back whe
                                                                                                * it's death and torque is use to 
                                                                                                * rotate the Top parts spin
                                                                                                */
  
    [SerializeField] private bool _applyKnockBack; //to apply knockBack effect & we can turn off if u want the dummy to stay where it is

    private float _currentHealth;   //store current health of dummy
    private float _knockBackStart;
    private int _playerFacingDirection;
    private bool _playerOnLeft;
    private bool _knockBack; //store it currently whether the player is knocked back or not

    private PlayerController _pc;   /*this will hold a ref to the playerController script attach to the player & will be used to get the 
                                     * the direction the player is facing when the enemy is damaged
                                     */
    //3 var to hold ref to the 3 child gameObject
    private GameObject _aliveGo, _brokenTopGo, _brokenBotGo;

    //3 variables to hold ref to the rb2D component of the 3 child gameObjects
    private Rigidbody2D _rbAlive, _rbBrokenTop, _rbBrokenBot;

    private Animator _aliveAnim;
    [SerializeField] private GameObject _hitParticles; 


    private void Start()
    {

        _currentHealth = _maxHealth;
        _pc = GameObject.Find("PlayerSprite").GetComponent<PlayerController>(); //it will go through hierarchy and find Player & get the PlayerController component

        _aliveGo = transform.Find("Alivee").gameObject;   /*here instead of loooking through the whole hierarchy it will just look at the
                                                          * children gameObjects of the Object that this script is attach to 
                                                          */
        _brokenTopGo = transform.Find("Broken Top").gameObject;
        _brokenBotGo = transform.Find("Broken Bottom").gameObject;

        _aliveAnim = _aliveGo.GetComponent<Animator>();
        _rbAlive = _aliveGo.GetComponent<Rigidbody2D>();
        _rbBrokenTop = _brokenTopGo.GetComponent<Rigidbody2D>();
        _rbBrokenBot = _brokenBotGo.GetComponent<Rigidbody2D>();

        _aliveGo.SetActive(true);
        _brokenTopGo.SetActive(false);
        _brokenBotGo.SetActive(false);
    }
    private void Update()
    {
        CheckKnockBack();
    }

    private void Damage(float[] details)
    {
        _currentHealth -= details[0];
        //_playerFacingDirection = _pc.GetFacingDirection();
        if (details[0] < _aliveGo.transform.position.x)
        {
            _playerFacingDirection = 1; //means player facing right
        }
        else
        {
            _playerFacingDirection = -1;
        }

        if (_hitParticles != null)
        {
            Instantiate(_hitParticles, _aliveGo.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        }
        else
        {
            Debug.Log("_hitParticle prefab not asssigned");
        }

        if (_playerFacingDirection == 1)
        {
            _playerOnLeft = true;
        }
        else
        {
            _playerOnLeft = false;
        }

        _aliveAnim.SetBool("playerOnLeftAnimatorParam", _playerOnLeft);
        _aliveAnim.SetTrigger("damageAnimatorParam");

        if (_applyKnockBack && _currentHealth > 0.0f)
        {
            //applyKnockBack
            KnockBack();
        }
        if (_currentHealth < 0.0f)
        {
            //die
            Die();
        }
    }

    private void KnockBack()
    {
        _knockBack = true;
        _knockBackStart = Time.time;
        _rbAlive.linearVelocity = new Vector2(_knockBackSpeedX * _playerFacingDirection, _knockBackSpeedY);
    }

    //this fun will take care of stopping the knockBack once a certain amount of time is passed
    private void CheckKnockBack()
    {
        if (Time.time >= _knockBackStart + _knockBackDuration && _knockBack)
        {
            _knockBack = false;
            _rbAlive.linearVelocity = new Vector2(0.0f, _rbAlive.linearVelocity.y);
        }
    }

    //fun to enabling the broken gameObjects and disabling the alive one
    private void Die()
    {
        _aliveGo.SetActive(false);
        _brokenTopGo.SetActive(true);
        _brokenBotGo.SetActive(true);

        //set the transform of broken parts to initial _aliveGo position
        _brokenTopGo.transform.position = _aliveGo.transform.position;
        _brokenBotGo.transform.position = _aliveGo.transform.position;

        //the top and bottom part's get diff force for knock back (optional just to effect style)
        _rbBrokenTop.linearVelocity = new Vector2(_knockBackDeathSpeedX * _playerFacingDirection, _knockBackDeathSpeedY); /*this is so that the top part gets
                                                                                                                           * knock back by a different amt of
                                                                                                                           * force
                                                                                                                           */
        _rbBrokenBot.linearVelocity = new Vector2(_knockBackSpeedX * _playerFacingDirection, _knockBackDeathSpeedY);
        _rbBrokenTop.AddTorque(_deathTorque * _playerFacingDirection, ForceMode2D.Impulse); //cause the top part to rotate
    }
}





































