using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class GameSession : MonoBehaviour
{
    [SerializeField] Image shipLivesIcon;
    [SerializeField] GameObject gameMenu;
    [SerializeField] bool gameMenuActive = false;
    [SerializeField] TMP_Text healthText;
    [SerializeField] GameObject levelCompletedCanvas;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] int totalLevelEnemies = 0;
    [SerializeField] bool unlockedNextLevel = false;
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] int currentNextLevel;
    [SerializeField] DisplayScores displayScores;
    private int score = 0;
    private void Start()
    {
        GetTotalNumberOfEnemies();
        UpdateShipLivesIconSprite();
    }
    private void Update()
    {
        ToggleGameMenu();
        CheckForLevelCompleted();
    }
    private void GetTotalNumberOfEnemies()
    {
        totalLevelEnemies = enemySpawner.GetNumberOfEnemies();
    }
    public void SubstractFromTotalLevelEnemies()
    {
        totalLevelEnemies -= 1;
    }
    private void CheckForLevelCompleted()
    {
        if (unlockedNextLevel == false && totalLevelEnemies <= 0)
        {
            unlockedNextLevel = true;

            levelCompletedCanvas.SetActive(true);
            Time.timeScale = 0;
            currentNextLevel = SceneManager.GetActiveScene().buildIndex;
            PlayerPrefs.SetInt("Level " + currentNextLevel.ToString(), 1);
            displayScores.CheckIfMadeBestScore();
        }
    }
    public void ActivateGameOverCanvas()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
    }    
    private void ToggleGameMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameMenuActive = !gameMenuActive;

            if (gameMenuActive == true)
            {
                gameMenu.SetActive(true);
            }    
            else
            {
                gameMenu.SetActive(false);
            }    
        }    
    }
    public void SetGameMenuBoolState(bool state)
    {
        gameMenuActive = state;
    }
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }
    public int GetScore()
    {
        return score;
    }
    private void UpdateShipLivesIconSprite()
    {
        GameMngr gameMngr = FindObjectOfType<GameMngr>();
        if (gameMngr != null)
        {
            shipLivesIcon.sprite = gameMngr.GetCurrentShipSprite();
        }
    }
    public void SetPlayerHealthText(float playerCurrentHealth)
    {
        healthText.text = playerCurrentHealth.ToString();
    }
}
