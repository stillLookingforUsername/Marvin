using UnityEngine;

public class Ladder : MonoBehaviour
{
    private float _vertical;
    private float _speed = 8f;
    private bool _isLadder;
    private bool _isClimbing;

    [SerializeField] private Rigidbody2D _rb;

    private void FixedUpdate()
    {
        if (_isClimbing)
        {
            _rb.gravityScale = 0f;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _vertical * _speed);
        }
        else
        {
            _rb.gravityScale = 3f;
        }
    }

    void Update()
    {
        _vertical = Input.GetAxis("Vertical");

        if (_isLadder && Mathf.Abs(_vertical) > 0)
        {
            _isClimbing = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            _isLadder = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            _isLadder = false;
            _isClimbing = false;
        }
    }
}
