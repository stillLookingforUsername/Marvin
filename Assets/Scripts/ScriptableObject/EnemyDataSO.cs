using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    public float chaseSpeed;
    public float jumpForce;
    public int damage;

}
