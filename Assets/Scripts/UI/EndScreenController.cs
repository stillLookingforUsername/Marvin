using UnityEngine;
using UnityEngine.SceneManagement;

namespace Marvin.UI
{
    public class EndScreenController : MonoBehaviour
    {
        [Header("Navigation")]
        [SerializeField] private string homeSceneName = "Home";

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