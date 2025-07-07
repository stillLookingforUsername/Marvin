using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Movement")]
    public float chaseSpeed = 3f;
    public float detectionRadius = 5f;
    public float attackRadius = 1.5f;
    
    [Header("Combat")]
    public int damage = 10;
    public float attackCooldown = 1f;
    public float maxHealth = 100f;
}
