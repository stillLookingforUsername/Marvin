using UnityEngine;

public class DestroyBox : MonoBehaviour
{
    public GameObject bullet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("DestroyBox");
            Destroy(this.gameObject);
        }
    }
}
