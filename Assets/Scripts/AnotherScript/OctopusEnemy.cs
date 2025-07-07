using UnityEngine;

public class OctopusEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyDataSO enemyData;
    private Transform player;
    private Rigidbody2D rb;
    private float currentHealth;
    private float lastAttackTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = enemyData.maxHealth;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // If player is within detection radius
        if (distanceToPlayer <= enemyData.detectionRadius)
        {
            // Move towards player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * enemyData.chaseSpeed;

            // Flip sprite based on movement direction
            if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);

            // If within attack radius and cooldown is over, attack
            if (distanceToPlayer <= enemyData.attackRadius && Time.time >= lastAttackTime + enemyData.attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            // Stop moving if player is out of range
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Attack()
    {
        // Try to get player's health component and deal damage
        if (player.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(enemyData.damage);
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Add death effects or drop items here
        Destroy(gameObject);
    }

    // Optional: Draw detection and attack radius gizmos in editor
    private void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.detectionRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.attackRadius);
    }
} 