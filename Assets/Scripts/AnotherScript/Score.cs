using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private TextMeshProUGUI _currentcoinText;
    [SerializeField] private TextMeshProUGUI _highScore;

    private int _cnt;

    private void Start()
    {
        _cnt = PlayerPrefs.GetInt("HighScore", 0);
        _currentcoinText.text = _cnt.ToString();
        _highScore.text = _cnt.ToString();
    }

    private void UpdateHighScore()
    {
        if (_cnt > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", _cnt);
            _highScore.text = _cnt.ToString();
        }
    }

    public void UpdateScore()
    {
        _cnt++;
        _currentcoinText.text = _cnt.ToString();
        //SoundManager.PlaySound(SoundType.COIN);
        UpdateHighScore();
    }

    // Called by ShopManager when coins are spent
    public void UpdateHighScoreFromShop(int newAmount)
    {
        _cnt = newAmount;
        _currentcoinText.text = _cnt.ToString();
        _highScore.text = _cnt.ToString();
    }
}
