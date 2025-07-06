using UnityEngine;

public class CoinIncreaseScore : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Score.Instance.UpdateScore();
            Destroy(gameObject);
        }
    }
}
