using TMPro;
using UnityEngine;

namespace Marvin.UI
{
    /// <summary>
    /// Displays the high score in GameOverScene
    /// </summary>
    public class HighScoreDisplay : MonoBehaviour
    {
        [Header("High Score Display")]
        [SerializeField] private TextMeshProUGUI _highScoreText;
        [SerializeField] private string _highScorePrefix = "High Score: ";
        [SerializeField] private bool _showPrefix = true;
        
        [Header("Debug")]
        [SerializeField] private bool _enableDebugLogs = true;

        private void Start()
        {
            DisplayHighScore();
        }

        /// <summary>
        /// Displays the current high score
        /// </summary>
        public void DisplayHighScore()
        {
            if (_highScoreText == null)
            {
                Debug.LogError("HighScoreDisplay: High Score TextMeshProUGUI is not assigned!");
                return;
            }

            int highScore = GetHighScore();
            string displayText = _showPrefix ? _highScorePrefix + highScore.ToString() : highScore.ToString();
            
            _highScoreText.text = displayText;
            
            if (_enableDebugLogs)
            {
                Debug.Log($"HighScoreDisplay: Displaying high score: {highScore}");
            }
        }

        /// <summary>
        /// Gets the high score from PlayerPrefs
        /// </summary>
        private int GetHighScore()
        {
            return PlayerPrefs.GetInt("HighScore", 0);
        }

        /// <summary>
        /// Updates the display with current high score
        /// </summary>
        public void RefreshDisplay()
        {
            DisplayHighScore();
        }

        /// <summary>
        /// Test method to manually refresh the display
        /// </summary>
        [ContextMenu("Test Refresh Display")]
        public void TestRefreshDisplay()
        {
            Debug.Log("HighScoreDisplay: Test refresh called");
            RefreshDisplay();
        }
    }
}