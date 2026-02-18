using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] Image musicButtonImage;
    [SerializeField] bool musicPlayerState = true;
    [SerializeField] Sprite musicButtonOnSprite;
    [SerializeField] Sprite musicButtonOffSprite;

    private void Start()
    {
        Singleton();
        RememberButtonState();
    }
    private void Singleton()
    {
        if (FindObjectsOfType<MusicPlayer>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    public void ToggleMusicButton()
    {
        musicPlayerState = !musicPlayerState;

        if (musicPlayerState == true)
        {
            musicPlayer.volume = 1;
            musicButtonImage.sprite = musicButtonOnSprite;
            PlayerPrefsMngr.SetSoundButtonState(1);
        }
        else
        {
            musicPlayer.volume = 0;
            musicButtonImage.sprite = musicButtonOffSprite;
            PlayerPrefsMngr.SetSoundButtonState(0);
        }
    }
    private void RememberButtonState()
    {
        int on = 1;
        if (PlayerPrefsMngr.GetSoundButtonState() == on)
        {
            musicPlayerState = true;
            musicPlayer.volume = 1;
            musicButtonImage.sprite = musicButtonOnSprite;
        }
        else
        {
            musicPlayerState = false;
            musicPlayer.volume = 0;
            musicButtonImage.sprite = musicButtonOffSprite;
        }
    }
}
