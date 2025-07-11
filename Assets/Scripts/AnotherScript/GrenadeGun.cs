using UnityEngine;
using UnityEngine.InputSystem;

public class GrenadeGun : MonoBehaviour
{
     // Direction = Destination - Orgin
    private Vector2 _direction;
    [SerializeField] private GameObject _gun;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;

    private GameObject _bulletInst;

    //we want to grab the world point of the mouse position & store it in some sort of vector2
    private Vector2 worldPosition;
    private void Update()
    {
        HandleGunRotation();
        HandleGunShooting();
    }
    private void HandleGunRotation()
    {
        //rotate the gun towards the mouse position
        worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        

        _direction = (worldPosition - (Vector2)_gun.transform.position).normalized; //need to normalize to get the proper direction
        _gun.transform.right = _direction;

        //flip the gun when it reaches a 90 degree threshold
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

        Vector3 localScale = new Vector3(1f, 1f, 1f);
        if (angle > 90 || angle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = 1f;
        }
        _gun.transform.localScale = localScale;
    }

    private void HandleGunShooting()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _bulletInst = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, _gun.transform.rotation);
        }
    }
    
}
