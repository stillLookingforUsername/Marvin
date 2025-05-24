using UnityEngine;

public class Gun : MonoBehaviour
{
    public float gunActiveDuration = 100f;
    private bool _isGunActive = false;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            //give the gun to the player
            PlayerGun playerGun = hitInfo.GetComponent<PlayerGun>();
            if (playerGun != null)
            {
                //enale the gun for a particular duration
                playerGun.EnableGunForDuration(gunActiveDuration);

                //after the duration is over destroy the gameObject
                Destroy(this.gameObject);
            }

        }
    }

}
