using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private int _progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;

    private int _currentLevel = 0;


    private void Start()
    {
        _progressAmount = 0;
        progressSlider.value = 0;
        Coins.OnCoinCollect += IncreaseProgress;
        HoldToLoad.OnHoldComplete += LoadNextLevel;
        loadCanvas.SetActive(true);
    }
    private void IncreaseProgress(int amt)
    {
        _progressAmount += amt;
        progressSlider.value = _progressAmount;

        if (_progressAmount >= 100)
        {
            loadCanvas.SetActive(true);
            Debug.Log("Level completed");
        }
    }
    private void LoadNextLevel()
    {
        int nextLevel = (_currentLevel == levels.Count - 1) ? 0 : _currentLevel + 1;
        loadCanvas.SetActive(false);

        levels[_currentLevel].gameObject.SetActive(false);
        levels[nextLevel].gameObject.SetActive(true);

        player.transform.position = new Vector3(0, 0, 0);

        _currentLevel = nextLevel;
        _progressAmount = 0;
        progressSlider.value = 0;

    }
}
