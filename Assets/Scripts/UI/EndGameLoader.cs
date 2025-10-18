using UnityEngine;
using UnityEngine.SceneManagement;

namespace Marvin.UI
{
    public static class EndGameLoader
    {
        public static void LoadEndScene(string endSceneName)
        {
            if (string.IsNullOrWhiteSpace(endSceneName))
            {
                Debug.Log("End scene name is empty");
                return;
            }

            if (!IsSceneInBuildSettings(endSceneName))
            {
                Debug.Log($"End Scene '{endSceneName}");
                return;
            }

            SceneManager.LoadScene(endSceneName, LoadSceneMode.Single);
        }

        private static bool IsSceneInBuildSettings(string sceneName)
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for(int i=0;i<sceneCount;i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                if(string.Equals(fileName, sceneName, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }
        
    }
}