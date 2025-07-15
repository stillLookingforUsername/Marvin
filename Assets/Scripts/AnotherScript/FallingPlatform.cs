using System.Collections;
using UnityEngine;
using DG.Tweening;



public class FallingPlatform : MonoBehaviour

{
    private float _fallWait = 1f;
    private float _destroyWait = 2f;

    private Tween _vibrationTween;


    private bool _isFalling;
    private Rigidbody2D _rb;
    private AudioSource _audioSource;



    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }


    private void OnCollisionEnter2D(Collision2D collision)

    {
        if (!_isFalling && collision.gameObject.CompareTag("Player"))
        {
            _audioSource.Play();
            _vibrationTween?.Kill();
            _vibrationTween = transform.DOShakePosition(
                1.0f,             // duration
                new Vector3(0.1f,0f,0f),             // strength
                30,               // vibrato
                0,               // randomness
                false,            // snapping
                true              // fade out
            );
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

