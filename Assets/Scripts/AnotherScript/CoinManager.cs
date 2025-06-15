using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public int coinCnt;

    private void Update()
    {
        coinText.text = ": " + coinCnt.ToString();
    }
}
