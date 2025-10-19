using UnityEngine;
using System;

public class TriggerBlock : MonoBehaviour
{
    public static event Action OnPlayerTriggered; // static so anyone can listen

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            OnPlayerTriggered?.Invoke(); // fire the event
        }
    }
}
