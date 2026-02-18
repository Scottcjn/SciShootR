using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    [SerializeField] EnemySpawner enemySpawner;
    public void LoadLevelsScene()
    {
        ResetGameDefaultValues();
        SceneManager.LoadScene("LevelsScene");
    }
    public void LoadMainMenuScene()
    {
        ResetGameDefaultValues();
        SceneManager.LoadScene("MainMenu");
    }
    public void RestartCurrentScene()
    {
        ResetGameDefaultValues();

        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void ResetGameDefaultValues()
    {
        Time.timeScale = 1;
    }
}
