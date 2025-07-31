using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth
    {
        get {return maxHealth;}
    }
    [SerializeField] private float invincibilityDuration = 1f;
    
    [Header("Events")]
    public UnityEvent<float> onHealthChanged;
    public UnityEvent onPlayerDeath;

    public float currentHealth;
    private bool isInvincible;
    private float invincibilityTimer;

    private void Start()
    {
        currentHealth = maxHealth;
        // Trigger initial health update
        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        onHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Start invincibility
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    private void Die()
    {
        onPlayerDeath?.Invoke();
        // You can add more death logic here

        Debug.Log("Player died!");
        gameObject.SetActive(false);
    }
} 