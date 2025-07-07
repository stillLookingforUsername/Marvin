using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 5f;

    private float _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
    }
    public void Damage(float damageAmount)
    {
        _currentHealth -= damageAmount;

        if (_currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
