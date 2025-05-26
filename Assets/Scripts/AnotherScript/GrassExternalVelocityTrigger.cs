using System.Collections;
using UnityEditor.XR;
using UnityEngine;

public class GrassExternalVelocityTrigger : MonoBehaviour
{
    private GrassInterationController _grassInterationController;

    private GameObject _player;
    private Material _material;
    private Rigidbody2D _rb;

    private bool _easeInCoroutineRunning;
    private bool _easeOutCoroutineRunning;

    private int _externalInfluence = Shader.PropertyToID("_ExternalInfluence");

    private float _startingXVelocity;
    private float _velocityLastFrame;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _rb = _player.GetComponent<Rigidbody2D>();
        _grassInterationController = GetComponentInParent<GrassInterationController>();

        _material = GetComponent<SpriteRenderer>().material;
        _startingXVelocity = _material.GetFloat(_externalInfluence);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.gameObject == _player)
        {
            if (!_easeInCoroutineRunning && Mathf.Abs(_rb.linearVelocity.x) > Mathf.Abs(_grassInterationController.velocityThreshold))
            {
                StartCoroutine(EaseIn(_rb.linearVelocity.x * _grassInterationController.externalInfluenceStrength));
            }
        }
    }
    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.gameObject == _player)
        {
            StartCoroutine(EaseOut());
        }
    }
    private void OnTriggerStay2D(Collider2D hitInfo)
    {
        if (hitInfo.gameObject == _player)
        {
            if (Mathf.Abs(_velocityLastFrame) > Mathf.Abs(_grassInterationController.velocityThreshold) &&
                Mathf.Abs(_rb.linearVelocity.x) < Mathf.Abs(_grassInterationController.velocityThreshold))
            {
                StartCoroutine(EaseOut());
            }
            else if (Mathf.Abs(_velocityLastFrame) < Mathf.Abs(_grassInterationController.velocityThreshold) &&
                Mathf.Abs(_rb.linearVelocity.x) > Mathf.Abs(_grassInterationController.velocityThreshold))
            {
                StartCoroutine(EaseIn(_rb.linearVelocity.x * _grassInterationController.externalInfluenceStrength));
            }
            else if (!_easeInCoroutineRunning && !_easeOutCoroutineRunning &&
                Mathf.Abs(_rb.linearVelocity.x) > Mathf.Abs(_grassInterationController.velocityThreshold))
            {
                _grassInterationController.InfluenceGrass(_material, _rb.linearVelocity.x * _grassInterationController.externalInfluenceStrength);
            }
            _velocityLastFrame = _rb.linearVelocity.x;
        }
    }

    private IEnumerator EaseIn(float XVelocity)
    {
        _easeInCoroutineRunning = true;

        float elapseTime = 0f;
        while (elapseTime < _grassInterationController.easeInTime)
        {
            elapseTime += Time.deltaTime;
            float lerpAmount = Mathf.Lerp(_startingXVelocity, XVelocity, (elapseTime / _grassInterationController.easeInTime));
            _grassInterationController.InfluenceGrass(_material, lerpAmount);

            yield return null;
        }
        _easeInCoroutineRunning = false;
    }

    private IEnumerator EaseOut()
    {
        _easeOutCoroutineRunning = true;
        float currentXInfluence = _material.GetFloat(_externalInfluence);

        float elapseTime = 0f;
        while (elapseTime < _grassInterationController.easeOutTime)
        {
            elapseTime += Time.deltaTime;

            float lerpAmount = Mathf.Lerp(currentXInfluence, _startingXVelocity, (elapseTime / _grassInterationController.easeOutTime));
            _grassInterationController.InfluenceGrass(_material, lerpAmount);

            yield return null;
        }
        _easeOutCoroutineRunning = false;
    }

}
