using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    public bool isPickedUp;
    private Vector2 vel;
    public float smoothTime;

    private void Update()
    {
        if (isPickedUp)
        {
            Vector3 offset = new Vector3(0f, 0.6f, 0f);
            transform.position = Vector2.SmoothDamp(transform.position, _player.transform.position + offset, ref vel, smoothTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isPickedUp)
        {
            isPickedUp = true;
        }
    }

}
