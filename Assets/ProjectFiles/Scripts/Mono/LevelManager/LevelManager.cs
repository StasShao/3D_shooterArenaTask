using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectFiles.Scripts.Mono.LevelManager
{
    public class LevelManager:MonoBehaviour
    {
        [SerializeField]private string gameSceneName;
        public void RestartLevel()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}