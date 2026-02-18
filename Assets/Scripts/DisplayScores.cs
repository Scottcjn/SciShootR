using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DisplayScores : MonoBehaviour
{
    [SerializeField] bool best = false;

    [SerializeField] TMP_Text currentScore;
    [SerializeField] TMP_Text bestScore;

    [SerializeField] GameSession gameSession;
    private string bestScoreKey = "Best Score Key";

    void Start()
    {
        CurrentScoreText();
        BestScoreText();
    }

    private void CurrentScoreText()
    {
        if (best == false)
        {
            currentScore.text = gameSession.GetScore().ToString();
        }
    }
    private void BestScoreText()
    {
        if (best == true)
        {
            if (gameSession.GetScore() > PlayerPrefs.GetInt(bestScoreKey, 0))
            {
                bestScore.text = gameSession.GetScore().ToString();
                PlayerPrefs.SetInt(bestScoreKey, gameSession.GetScore());
            }
            else
            {
                bestScore.text = PlayerPrefs.GetInt(bestScoreKey, 0).ToString();
            }
        }
    }
    private void Update()
    {
        if (best == false)
        {
            currentScore.text = gameSession.GetScore().ToString();
        }
    }
    public void CheckIfMadeBestScore()
    {
        if (gameSession.GetScore() > PlayerPrefs.GetInt(bestScoreKey, 0))
        {
            PlayerPrefs.SetInt(bestScoreKey, gameSession.GetScore());
        }
    }
}
