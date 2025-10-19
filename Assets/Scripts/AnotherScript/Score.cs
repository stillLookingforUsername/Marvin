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
            DontDestroyOnLoad(gameObject);
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

    /// <summary>
    /// Gets the current high score from PlayerPrefs
    /// </summary>
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }
    
    /// <summary>
    /// Gets the current score count
    /// </summary>
    public int GetCurrentScore()
    {
        return _cnt;
    }
    
    /// <summary>
    /// Updates high score display for GameOverScene
    /// </summary>
    public void UpdateHighScoreDisplay()
    {
        int highScore = GetHighScore();
        if (_highScore != null)
        {
            _highScore.text = highScore.ToString();
        }
    }
    
    /// <summary>
    /// Sets the high score TextMeshProUGUI reference (for GameOverScene)
    /// </summary>
    public void SetHighScoreDisplay(TextMeshProUGUI highScoreText)
    {
        _highScore = highScoreText;
        UpdateHighScoreDisplay();
    }
    
    /// <summary>
    /// Resets the current score (useful when starting a new game)
    /// </summary>
    public void ResetCurrentScore()
    {
        _cnt = 0;
        if (_currentcoinText != null)
        {
            _currentcoinText.text = _cnt.ToString();
        }
    }


    
}
