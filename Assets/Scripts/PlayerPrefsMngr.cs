using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsMngr : MonoBehaviour
{
    private const string BEST_SCORE_KEY = "Best Score Key";
    private const string MUSIC_BUTTON_KEY = "Music Button Key";
    private const string SFXS_BUTTON_KEY = "Sfxs Button Key";
    public static void SetNewBestScore(int newBestScore)
    {
        PlayerPrefs.SetInt(BEST_SCORE_KEY, newBestScore);
    }
    public static int GetNewBestScore()
    {
        int defaultBestScore = 0;
        return PlayerPrefs.GetInt(BEST_SCORE_KEY, defaultBestScore);
    }

    public static void SetMusicButtonState(int buttonState)
    {
        PlayerPrefs.SetInt(MUSIC_BUTTON_KEY, buttonState);
    }
    public static int GetMusicButtonState()
    {
        int defaultSoundButtonState = 1;
        return PlayerPrefs.GetInt(MUSIC_BUTTON_KEY, defaultSoundButtonState);
    }

    public static void SetSfxsButtonState(int buttonState)
    {
        PlayerPrefs.SetInt(SFXS_BUTTON_KEY, buttonState);
    }
    public static int GetSfxsButtonState()
    {
        int defaultVolumeButtonState = 1;
        return PlayerPrefs.GetInt(SFXS_BUTTON_KEY, defaultVolumeButtonState);
    }
}
