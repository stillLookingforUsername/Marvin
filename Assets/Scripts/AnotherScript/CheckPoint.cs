using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.UpdateCheckPoint(respawnPoint.position);
        }
    }


}
