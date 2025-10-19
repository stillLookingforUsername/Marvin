using UnityEngine;

public class Straberry : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            Score.Instance.UpdateScore();
            Destroy(gameObject);
        }
    }
}
