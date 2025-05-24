using UnityEngine;

public class Collector : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        InterfaceCollector item = collision.GetComponent<InterfaceCollector>();
        if (item != null)
        {
            item.Collect();
        }
    }
}
