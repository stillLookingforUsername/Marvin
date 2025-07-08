using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField, Range(5f, 30f)] private float bounceForce = 15f;
    [SerializeField] private bool resetJumpsOnBounce = true;
    [SerializeField] private bool resetDashesOnBounce = true;
    
    [Header("Damage Settings")]
    [SerializeField, Range(1f, 50f)] private float damageAmount = 10f;
    [SerializeField] private Color trapColor = Color.red;
    
    [Header("Effects")]
    [SerializeField] private bool useVisualFeedback = true;
    [SerializeField] private float flashDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            HandleTrapEffect(player);
        }
    }

    private void HandleTrapEffect(GameObject player)
    {
        // Get required components
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerMovement == null || playerHealth == null)
        {
            Debug.LogWarning("Required components not found on player!");
            return;
        }

        // Apply damage
        playerHealth.TakeDamage(damageAmount);

        // Apply bounce effect by forcing a jump
        if (resetJumpsOnBounce)
        {
            playerMovement.SendMessage("ResetJumpValues", SendMessageOptions.DontRequireReceiver);
        }
        if (resetDashesOnBounce)
        {
            playerMovement.SendMessage("ResetDashes", SendMessageOptions.DontRequireReceiver);
        }

        // Set vertical velocity through the PlayerMovement component
        playerMovement.VerticalVelocity = bounceForce;

        // Visual feedback
        if (useVisualFeedback && spriteRenderer != null)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        spriteRenderer.color = trapColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    // Optional: Visualize the trap's trigger area in the editor
    private void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }
} 