using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataContainer", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    public float horizontalSpeed;
    public float jumpForce;
}
