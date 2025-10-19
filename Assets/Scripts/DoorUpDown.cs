using UnityEngine;
using DG.Tweening;

public class DoorUpDown : MonoBehaviour
{
    [SerializeField] private float moveDistance = 1f;  // how far it moves up/down
    [SerializeField] private float duration = 1f;      // how long one move takes

    private void Start()
    {
        // Store the original position
        Vector3 startPos = transform.position;

        // Move up, then down, and loop forever
        transform.DOMoveY(startPos.y + moveDistance, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
