using UnityEngine;

//moto - the damage the player when it gets hit by the enemy
public class PlayerState : MonoBehaviour
{
    [SerializeField]
    private float _maxHealth;

    //to spawn some particles while getting damaged
    [SerializeField]
    private GameObject
        _deathChunkParticles,
        _deathBloodParticles;

    private float _currentHealth;
    private GameManager _gameManager;

    private void Start()
    {
        _currentHealth = _maxHealth;
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void DecreaseHealth(float damageAmt)
    {
        _currentHealth -= damageAmt;

        if (_currentHealth <= 0.0f)
        {
            Die();
        }
    }
    private void Die()
    {
        Instantiate(_deathChunkParticles, transform.position, _deathChunkParticles.transform.rotation);
        Instantiate(_deathBloodParticles, transform.position, _deathBloodParticles.transform.rotation);
        _gameManager.Respawn();     /*once the gameObject is destroyed the script will get destroyed with it, so we won't get to any Code
                                     *after this..., so that's why we need to call the Respawn() frm GameManager before we Destroy it,
                                     *that's why we destory after calling it
                                     */
        Debug.Log("Respawn is called");
        Destroy(gameObject);
    }


}
