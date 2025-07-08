using UnityEngine;
using UnityEngine.InputSystem;

public abstract class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer _interactSprite;

    //get the distance between the player and the NPC and if the distance is less than 2f, show the interact sprite
    private Transform _playerTransform;
    private const float INTERACT_RANGE = 2f;
    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        if(Keyboard.current.eKey.wasPressedThisFrame && IsWithinInteractRange())
        {
            //interact with NPC
            Interact();
        }
        if(_interactSprite.gameObject.activeSelf && !IsWithinInteractRange())
        {
            _interactSprite.gameObject.SetActive(false);
        }
        else if(!_interactSprite.gameObject.activeSelf && IsWithinInteractRange())
        {
            _interactSprite.gameObject.SetActive(true);
        }
    }
    public abstract void Interact();

    private bool IsWithinInteractRange()
    {
        if(Vector2.Distance(_playerTransform.position, transform.position) < INTERACT_RANGE)
        {
            return true;     
        }
        else
        {
            return false;
        }
    }
}
