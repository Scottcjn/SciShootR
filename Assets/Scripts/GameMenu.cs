using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameMenu : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] GameSession gameSession;
    [SerializeField] bool playMusic = true;
    [SerializeField] bool playSfxs = true;
    [SerializeField] Image musicButtonImage;
    [SerializeField] Image sfxsButtonImage;
    [SerializeField] Sprite toggleOnSprite;
    [SerializeField] Sprite toggleOffSprite;

    [SerializeField] AudioPlayer musicPlayer;
    [SerializeField] AudioPlayer sfxsPlayer;

    private void OnEnable()
    {
        PauseGame();
    }
    private void OnDisable()
    {
        ResumeGame();
    }
    private void Start()
    {
        RememberButtonSprites();
    }
    private void PauseGame()
    {
        player.SetCanShootState(false);
        Time.timeScale = 0;
    }
    private void ResumeGame()
    {
        player.SetCanShootState(true);
        Time.timeScale = 1;
    }
    public void ResumeGameButton()
    {
        player.SetCanShootState(true);
        Time.timeScale = 1;
        gameObject.SetActive(false);
        gameSession.SetGameMenuBoolState(false);
    }
    public void MainMenuButton()
    {
        sceneLoader.LoadMainMenuScene();
    }
    public void ToggleMusicButton()
    {
        playMusic = !playMusic;

        if (playMusic == true)
        {
            musicPlayer.TurnOnAudio();
            musicButtonImage.sprite = toggleOnSprite;
            PlayerPrefsMngr.SetMusicButtonState(1);
        }
        else
        {
            musicPlayer.TurnOffAudio();
            musicButtonImage.sprite = toggleOffSprite;
            PlayerPrefsMngr.SetMusicButtonState(0);
        }
    }
    public void ToggleSfxsButton()
    {
        playSfxs = !playSfxs;

        if (playSfxs == true)
        {
            sfxsPlayer.TurnOnAudio();
            sfxsButtonImage.sprite = toggleOnSprite;
            PlayerPrefsMngr.SetSfxsButtonState(1);
        }
        else
        {
            sfxsPlayer.TurnOffAudio();
            sfxsButtonImage.sprite = toggleOffSprite;
            PlayerPrefsMngr.SetSfxsButtonState(0);
        }
    }
    private void RememberButtonSprites()
    {
        GetValuesOfBools();
        if (playMusic == true)
        {
            musicButtonImage.sprite = toggleOnSprite;
        }
        else
        {
            musicButtonImage.sprite = toggleOffSprite;
        }
        if (playSfxs == true)
        {
            sfxsButtonImage.sprite = toggleOnSprite;
        }
        else
        {
            sfxsButtonImage.sprite = toggleOffSprite;
        }
    }
    private void GetValuesOfBools()
    {
        int on = 1;
        if (PlayerPrefsMngr.GetMusicButtonState() == on)
        {
            playMusic = true;
        }
        else
        {
            playMusic = false;
        }
        if (PlayerPrefsMngr.GetSfxsButtonState() == on)
        {
            playSfxs = true;
        }
        else
        {
            playSfxs = false;
        }
    }
}
