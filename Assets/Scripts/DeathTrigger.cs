using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Deal damage equal to max health → triggers onPlayerDeath event
                playerHealth.TakeDamage(playerHealth.MaxHealth);
            }
        }
    }
}