using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public AudioSource coinCollectSound;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public TextMeshProUGUI coinText;
    public int coinCnt;


    private void Update()
    {
        coinText.text = ": " + coinCnt.ToString();
    }

    public void AddCoin()
    {
        coinCnt++;
        coinCollectSound.Play();
    }

}
