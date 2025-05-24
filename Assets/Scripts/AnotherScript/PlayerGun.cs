using Mono.Cecil;
using System.Collections;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    public GameObject gunVisual;    //gun object to enable/disable
    private bool _gunActive = false;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [SerializeField]private AudioClip _fireSound;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void EnableGunForDuration(float duration)
    {
        if (_gunActive) return;
        _gunActive = true;
        gunVisual.SetActive(true);
        StartCoroutine(DisableGunAfterTime(duration));
    }

    private IEnumerator DisableGunAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        gunVisual.SetActive(false);
        _gunActive = false;
    }
    private void Update()
    {
        if (_gunActive && Input.GetKeyDown(KeyCode.T))
        {
            Fire();
            //play SoundFX
            _audioSource.clip = _fireSound;
            _audioSource.Play();
        }
    }

    private void Fire()
    {
        Debug.Log("Fire..........Fire");
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

    }
}
