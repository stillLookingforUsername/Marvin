using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    private Rigidbody2D rb;
    public GameObject bulletEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Debug.Log(hitInfo);
        Triangle triangle = hitInfo.GetComponent<Triangle>();
        if (triangle != null)
        {
            triangle.TakeDamage(damage);
        }
        Instantiate(bulletEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
