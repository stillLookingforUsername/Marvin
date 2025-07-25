using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FallBox : MonoBehaviour
{
    public float targetY = -4.0f;      // Where the object should stop
    public float fallDuration = 1.0f; // How long the fall lasts
    private void Start()
    {
        targetY = -4.0f;
    }

    private bool hasFallen = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasFallen && other.CompareTag("Bullet"))
        {
            hasFallen = true;

            // Start a downward fall animation
            transform.DOLocalMoveY(targetY, fallDuration)
                     .SetEase(Ease.InQuad); // Ease.InQuad simulates gravity
        }
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(1f);
    }

}
