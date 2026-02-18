using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMngr : MonoBehaviour
{
    [Header("player ships variables")]

    [SerializeField] Sprite currentShipSprite;
    [SerializeField] Sprite ship1;
    [SerializeField] Sprite ship2;
    [SerializeField] Sprite ship3;
    [SerializeField] Sprite ship4;

    [Header("Game Mngr Cached Variabbles")]

    [SerializeField] ButtonMngr buttonMngr;

    private string lastSelectedShipId = "lastSelectedShipIdKey";
    private string playersLeaderboardNameKey = "players Leaderboard Name Key";

    void Start()
    {
        Singleton();
        RememberLastSelectedShipSkin();

        PlayerPrefs.GetString(playersLeaderboardNameKey);
    }
    private void Singleton()
    {
        if (FindObjectsOfType<GameMngr>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    public void SetCurrentShipSprite(Sprite selectedShipSprite, Item item)
    {
        currentShipSprite = selectedShipSprite;
        PlayerPrefs.SetInt(lastSelectedShipId, item.shopItem.id);
    }
    public Sprite GetCurrentShipSprite()
    {
        return currentShipSprite;
    }
    private void RememberLastSelectedShipSkin()
    {
        if (PlayerPrefs.GetInt(lastSelectedShipId) == 0)
        {
            currentShipSprite = ship1;
        }
        else if (PlayerPrefs.GetInt(lastSelectedShipId) == 1)
        {
            currentShipSprite = ship2;
        }
        else if (PlayerPrefs.GetInt(lastSelectedShipId) == 2)
        {
            currentShipSprite = ship3;
        }
        else if (PlayerPrefs.GetInt(lastSelectedShipId) == 3)
        {
            currentShipSprite = ship4;
        }
    }
    public void SetPlayersLeaderboardName(string  playersName)
    {
        PlayerPrefs.SetString(playersLeaderboardNameKey, playersName);
    }
    public string GetPlayersLeaderboardName()
    {
        return PlayerPrefs.GetString(playersLeaderboardNameKey);
    }
}
