using Unity.Cinemachine;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public int health = 3;
    public GameObject deathEffect;
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile profile;

    [SerializeField] private AudioClip _clip;
    private AudioSource _source;
    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        _source = GetComponent<AudioSource>();
    }
    public void TakeDamage(int damage)
    {
        //CameraShakeManager.instance.CameraShake(impulseSource);
        CameraShakeManager.instance.ScreenShakeFromProfile(profile, impulseSource);
        health -= damage;
        //play soundFx
        _source.clip = _clip;
        _source.Play();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
