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

    public AudioSource coinCollectSound;
    private int _cnt;

    private void Start()
    {
        _currentcoinText.text = _cnt.ToString();

        _highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        UpdateHighScore();
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
        coinCollectSound.Play();
        UpdateHighScore();
    }
}
