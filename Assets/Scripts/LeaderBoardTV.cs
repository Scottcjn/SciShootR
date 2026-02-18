using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LeaderBoardTV : MonoBehaviour
{
    [SerializeField] TMP_InputField nameText;
    [SerializeField] GameMngr gameMngr;
    [SerializeField] TMP_Text bestScore;
    [SerializeField] GameObject nameButton;

    private string leaderboardLink = "http://dreamlo.com/lb/bA4a8huIkEehODtWLXnwxAggKv33gPXkqhkgsx5pUuAg";
    void Awake()
    {
        gameMngr = FindObjectOfType<GameMngr>();
        bestScore.text = PlayerPrefsMngr.GetNewBestScore().ToString();
    }
    private void OnEnable()
    {
        if (gameMngr.GetPlayersLeaderboardName() != "")
        {
            nameButton.SetActive(false);
        }
    }
    public void SaveNameAndScoreInLeaderboard()
    {
        if (nameText.text != "" || gameMngr.GetPlayersLeaderboardName() != "")
        {
            Debug.Log("entered function");
            CheckIfPlayerHasName();
            HighScores.UploadScore(gameMngr.GetPlayersLeaderboardName(), int.Parse(bestScore.text));
            Debug.Log("left function");
            nameButton.SetActive(false);
        }
    }
    private void CheckIfPlayerHasName()
    {
        if (gameMngr.GetPlayersLeaderboardName() == "")
        {
            gameMngr.SetPlayersLeaderboardName(nameText.text);
        }
    }
}
