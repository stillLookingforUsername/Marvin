using UnityEngine;

public class Block : MonoBehaviour
{
     private void OnEnable()
    {
        TriggerBlock.OnPlayerTriggered += DestroySelf;
    }

    private void OnDisable()
    {
        TriggerBlock.OnPlayerTriggered -= DestroySelf;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}