using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 3f;
    public int damageAmount = 10;




    private Vector3 nextPosition;

    private void Start()
    {
        nextPosition = pointA.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);

        if(transform.position == nextPosition)
        {
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if(playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }


}
