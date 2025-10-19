using DG.Tweening;
using UnityEngine;

public class IndicatorSystem : MonoBehaviour
{
    public enum MoveDirection { Left, Right, Up, Down }

    [Header("Movement Setting")]
    public MoveDirection moveDirection = MoveDirection.Right;
    public float moveDistance = 0.5f;
    public float moveDuration = 1f;
    public bool loop = true;

    [Header("Optional Effects")]
    public bool pulseScale = false;
    public float pulseAmount = 1.2f;
    public float pulseDuration = 0.5f;

    private Vector3 startPos;
    private Tween moveTween;
    private Tween pulseTween;


    private void Start()
    {
        startPos = transform.position;

        Vector3 directionVector = GetDirectionVector();
        Vector3 targetPos = startPos + directionVector * moveDistance;

        //Move tween
        moveTween = transform.DOMove(targetPos, moveDuration).SetEase(Ease.InOutSine);

        if (loop)
        {
            moveTween.SetLoops(-1, LoopType.Yoyo);
        }

        //Optional pulse
        if (pulseScale)
        {
            pulseTween = transform.DOScale(pulseAmount, pulseDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private Vector3 GetDirectionVector()
    {
        switch (moveDirection)
        {
            case MoveDirection.Left: return Vector3.left;
            case MoveDirection.Right: return Vector3.right;
            case MoveDirection.Up: return Vector3.up;
            case MoveDirection.Down: return Vector3.down;
            default: return Vector3.right;
        }
    }

    private void OnDestroy()
    {
        //clean up Tweens to prevent memory Leaks
        moveTween?.Kill();
        pulseTween?.Kill();
    }

}