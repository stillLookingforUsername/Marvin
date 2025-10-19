using UnityEngine;
using UnityEngine.SceneManagement;

namespace Marvin.UI
{
    public class EndScreenController : MonoBehaviour
    {
        [Header("Navigation")]
        [SerializeField] private string homeSceneName = "Home";

        [Header("High Score Display")]
        [SerializeField] private HighScoreDisplay _highScoreDisplay;

        private void Start()
        {
            // Ensure high score is displayed when the scene loads
            if (_highScoreDisplay != null)
            {
                _highScoreDisplay.RefreshDisplay();
            }
        }

        public void OnRetryClicked()
        {
            int activeIndex = SceneManager.GetActiveScene().buildIndex;
            if (activeIndex < 0)
            {
                Debug.Log("Active scene index invalid");
                return;
            }
            SceneManager.LoadScene(activeIndex, LoadSceneMode.Single);
        }

        public void OnHomeClicked()
        {
            if(string.IsNullOrWhiteSpace(homeSceneName))
            {
                Debug.Log("Home Scene is not Empty");
                return;
            }
            SceneManager.LoadScene(homeSceneName, LoadSceneMode.Single);
        }
    }
}