using Unity.Cinemachine;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shot();
        }
    }
    private void Shot()
    {
        Instantiate(bulletPrefab,firePoint.position, firePoint.rotation);
    }
}
