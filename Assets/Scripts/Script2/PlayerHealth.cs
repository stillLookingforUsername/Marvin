using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public Health healthUI;

    private void Start()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy)
        {
            TakeDamage(enemy.data.damage);
        }
    }
    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
