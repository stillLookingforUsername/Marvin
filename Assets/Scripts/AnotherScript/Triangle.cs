using Unity.Cinemachine;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public int health = 3;
    public GameObject deathEffect;
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile profile;

    [SerializeField] private AudioClip[] _damageSoundClip;
    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    public void TakeDamage(int damage)
    {
        //CameraShakeManager.instance.CameraShake(impulseSource);
        CameraShakeManager.instance.ScreenShakeFromProfile(profile, impulseSource);
        health -= damage;

        //play soundFx
        //SoundFXManager.Instance.PlaySoundFXClip(_damageSoundClip, transform, 1f);

        //select random sound from multi_sound
        //SoundFXManager.Instance.PlayRandomSoundFXClip(_damageSoundClip,transform,1f);

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
