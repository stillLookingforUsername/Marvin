using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public float chaseSpeed;
    public float jumpForce;
    public int damage;

}
