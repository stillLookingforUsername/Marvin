using UnityEngine;
using DG.Tweening;  // DOTween namespace

public class ScaleOverText : MonoBehaviour
{
    // Reference to the UI Text (or GameObject) you want to scale
    public RectTransform textTransform; // RectTransform if it's a UI Text
    public float scaleDuration = 0.5f;  // Duration for scaling
    public float targetScale = 1.5f;    // Target scale value (1.5 is 150% of original size)

    private void Start()
    {
        // Call ScaleUp and ScaleDown on start as an example
        ScaleUp();
    }

    // Scale up the text
    public void ScaleUp()
    {
        textTransform.DOScale(targetScale, scaleDuration).OnComplete(() => {
            // Once scaling up is done, you can scale it back down after a delay
            ScaleDown();
        });
    }

    // Scale down the text
    public void ScaleDown()
    {
        textTransform.DOScale(1f, scaleDuration);  // Scale back to original size (1)
    }
}

