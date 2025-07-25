using UnityEngine;

public class ActivateMovingPlatform : MonoBehaviour
{
    public MovingPlatform movingPlatform;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            movingPlatform.enabled = true;
            gameObject.SetActive(false);
        }
    }
}
