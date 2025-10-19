using UnityEngine;

public class DeathTriggerActivator : MonoBehaviour
{
    [SerializeField] private GameObject deathTrigger; // Assign your DeathTrigger object here

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            if (deathTrigger != null)
            {
                deathTrigger.SetActive(true);
                Debug.Log("DeathTriggerActivator: Death trigger activated!");
            }
            else
            {
                Debug.LogWarning("DeathTriggerActivator: Death trigger reference not set!");
            }
        }
    }
}
