using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private LayerMask _interactLayer;  //layer our grenade will interact with

    [SerializeField] private float _radius; // force Area where there will be the grenade Impact
    [SerializeField] private int _explosionForce;
    [SerializeField] private float _explosionTime;
    [SerializeField] private GameObject _explosionPrefab;
    public AudioSource src;
    public AudioClip boom;

    private float _explosionTimer;

    private void Start()
    {
        src.clip = boom;
    }

    private void FixedUpdate()
    {
        _explosionTimer += Time.fixedDeltaTime;

        if (_explosionTimer >= _explosionTime)
        {
            //do our explosion
            Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position,_radius,_interactLayer);

            foreach (Collider2D coll in collisions)
            {
                if (coll.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                {
                    Vector3 dir = rb.transform.position - transform.position;
                    rb.AddForce(dir.normalized * _explosionForce);
                }
            }

            //spawn explosion effects
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            src.Play();
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
