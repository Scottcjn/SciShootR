using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonMngr : MonoBehaviour
{
    [SerializeField] GameObject buttonCanvas;

    [SerializeField] bool playMusic = true;
    
    [SerializeField] Image musicButtonImage;
    [SerializeField] Sprite musicButtonOnSprite;
    [SerializeField] Sprite musicButtonOffSprite;
    [SerializeField] AudioPlayer musicPlayer;

    [SerializeField] bool playSfxs = true;

    [SerializeField] Image sfxsButtonImage;
    [SerializeField] Sprite sfxsButtonOnSprite;
    [SerializeField] Sprite sfxsButtonOffSprite;

    private void Start()
    {
        RememberMusicButtonState();
        RememberSfxsButtonState();
    }
    public void SetButtonCanvasActive()
    {
        buttonCanvas.SetActive(true);
    }
    public void SetButtonCanvasInactive()
    {
        buttonCanvas.SetActive(false);
    }
    public void ToggleMusicButton()
    {
        playMusic = !playMusic;

        if (playMusic == true)
        {
            musicPlayer.TurnOnAudio();
            musicButtonImage.sprite = musicButtonOnSprite;
            PlayerPrefsMngr.SetMusicButtonState(1);
        }
        else
        {
            musicPlayer.TurnOffAudio();
            musicButtonImage.sprite = musicButtonOffSprite;
            PlayerPrefsMngr.SetMusicButtonState(0);
        }
    }
    private void RememberMusicButtonState()
    {
        int on = 1;
        if (PlayerPrefsMngr.GetMusicButtonState() == on)
        {
            playMusic = true;
            musicButtonImage.sprite = musicButtonOnSprite;
        }
        else
        {
            playMusic = false;
            musicButtonImage.sprite = musicButtonOffSprite;
        }
    }
    public void ToggleSfxsButton()
    {
        playSfxs = !playSfxs;

        if (playSfxs == true)
        {
            sfxsButtonImage.sprite = sfxsButtonOnSprite;
            PlayerPrefsMngr.SetSfxsButtonState(1);
        }
        else
        {
            sfxsButtonImage.sprite = sfxsButtonOffSprite;
            PlayerPrefsMngr.SetSfxsButtonState(0);
        }
    }
    private void RememberSfxsButtonState()
    {
        int on = 1;
        if (PlayerPrefsMngr.GetSfxsButtonState() == on)
        {
            playSfxs = true;
            sfxsButtonImage.sprite = sfxsButtonOnSprite;
        }
        else
        {
            playSfxs = false;
            sfxsButtonImage.sprite = sfxsButtonOffSprite;
        }
    }
    public bool GetMusicPlayerBoolState()
    {
        return playMusic;
    }
    public bool GetSfxsPlayerBoolState()
    {
        return playSfxs;
    }
}







