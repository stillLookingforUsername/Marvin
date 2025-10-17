using UnityEngine;
using UnityEngine.SceneManagement;

namespace Margin.UnityEngine
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Navigation")]
        [SerializeField] private string nextSceneName = string.Empty;

        //call this from the Button's OnClick event in the Inspector
        public void OnPlayClicked()
        {
            if (!string.IsNullOrWhiteSpace(nextSceneName))
            {
                LoadByName(nextSceneName);
                return;
            }
            LoadNextByBuildIndex();
        }

        private static void LoadByName(string sceneName)
        {
            //Validate the scene is in Build Settings
            if (IsSceneInBuildSettings(sceneName))
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
            else
            {
                Debug.Log($"Scene '{sceneName}' is not added to Build Settgins.");
            }
        }

        private static void LoadNextByBuildIndex()
        {
            int activeIndex = SceneManager.GetActiveScene().buildIndex;
            int totalScenes = SceneManager.sceneCountInBuildSettings;

            if (totalScenes <= 0)
            {
                Debug.Log("No scenes found");
                return;
            }

            int nextIndex = activeIndex + 1;
            if (nextIndex >= totalScenes)
            {
                Debug.LogError("There is no 'next' scene");
                return;
            }

            SceneManager.LoadScene(nextIndex, LoadSceneMode.Single);
        }

        private static bool IsSceneInBuildSettings(string sceneName)
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for(int i = 0;i < sceneCount;i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (string.Equals(fileName, sceneName, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }
    }
}