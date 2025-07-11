using UnityEngine;

public class RevolverActivate : MonoBehaviour
{
    public GameObject revolver;
    public PlayerAimShoot playerAimShoot;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            revolver.SetActive(true);
            playerAimShoot.enabled = true;
            Destroy(gameObject);
        }
    }
}
