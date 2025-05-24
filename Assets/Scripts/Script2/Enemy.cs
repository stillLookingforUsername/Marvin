using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeEditor;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
    public EnemyData data;
    public Transform player;
    /*
    public float chaseSpeed = 2f;
    public float jumpForce = 2f;
    */
    public LayerMask groundLayer;
    //public int damage = 1;

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private bool _shouldJump;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }

        }
    }
    private void Update()
    {
        //is grounded
        _isGrounded = Physics2D.Raycast(transform.position, Vector3.down, 1f, groundLayer);

        //Player direction
        float playerDirection = Mathf.Sign(player.position.x - transform.position.x);

        //player Above
        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector3.up, 3f, 1 << player.gameObject.layer);

        if (_isGrounded)
        {
            _rb.linearVelocity = new Vector2(playerDirection * data.chaseSpeed, _rb.linearVelocity.y);   //chase player
            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(playerDirection, 0), 2f, groundLayer);   //check ground in Front
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(playerDirection, 0, 0), Vector2.down, groundLayer);  //check gap Ahead
            /*new Vector3(playerDirection, 0, 0) creates a small offset in the horizontal direction
             * (either +1 or -1 unit along the x-axis) based on where the player is.
             * Adding these together shifts the starting point of the downward raycast
             * a bit in front of the enemy.
            */

            RaycastHit2D groundAbove = Physics2D.Raycast(transform.position, Vector2.up, 4f, groundLayer);  //check ground above

            if (!groundInFront.collider && !gapAhead)
            {
                _shouldJump = true;
            }
            else if (isPlayerAbove && groundAbove.collider)
            {
                _shouldJump = true;
            }
        }

    }
    private void FixedUpdate()
    {
        if (_isGrounded && _shouldJump)
        {
            _shouldJump = false;
            Vector2 enemyMoveToward = (player.position - transform.position).normalized;
            Vector2 enemyJumpToward = enemyMoveToward * data.jumpForce;
            _rb.AddForce(new Vector2(enemyJumpToward.x, data.jumpForce), ForceMode2D.Impulse);
        }
    }


}

