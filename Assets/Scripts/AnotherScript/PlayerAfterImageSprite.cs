using TMPro;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    //how long it has been Active;
    [SerializeField] private float _activeTime = 0.1f;
    private float _timeActivated;
    private float _alpha;   //what the current alpha is

    //[SerializeField] private float _alphaSet = 2f;     //set the alpha when we enable the gameObject
    [SerializeField] private float _alphaSet = 1f;     //set the alpha when we enable the gameObject
    //private float _alphaMultiplier = 10f;     //decrease the alpha overTime, smaller the no fast the sprite will fade
    [SerializeField] private float _alphaDecay = 0.9f;     //decrease the alpha overTime, smaller the no fast the sprite will fade

    private Transform _player;

    private SpriteRenderer _sr;
    private SpriteRenderer _playerSR;

    private Color _color;

    //it gets called everytime we enable the gameObject
    private void OnEnable() 
    {
        //reference the the spriteRenderer component of this gameObject
        _sr = GetComponent<SpriteRenderer>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;  //get reference to the transform of the player gameOject
        _playerSR = GetComponent<SpriteRenderer>();     //ref to the SpriteRenderer component of the Player gameObject

        _alpha = _alphaSet;
        _sr.sprite = _playerSR.sprite;
        transform.position = _player.position;    //set the gameObject's transform to the Player transform
        transform.rotation = _player.rotation;
        _timeActivated = Time.time;
    }

    private void FixedUpdate()
    {
        //_alpha *= _alphaDecay;      //start off by decreasing the alpha my multiplying with _alphaMultiplier
        _alpha -= _alphaDecay * Time.deltaTime;      //start off by decreasing the alpha my multiplying with _alphaMultiplier
        _color = new Color(1f, 1f, 1f, _alpha);
        _sr.color = _color;     //set the color to our new color _color;

        //to check if our afterImage has been activated for long enough

        if (Time.time >= (_timeActivated + _activeTime))
        {
            //Add back to the pool
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
