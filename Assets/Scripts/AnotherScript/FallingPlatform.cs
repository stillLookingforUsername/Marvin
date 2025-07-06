using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private float _fallWait = 1f;
    private float _destroyWait = 2f;

    private bool _isFalling;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isFalling && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        _isFalling = true;
        yield return new WaitForSeconds(_fallWait);
        _rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, _destroyWait);
    }

}
