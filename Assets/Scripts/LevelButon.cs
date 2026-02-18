using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelButon : MonoBehaviour
{
    [Header("Variables to edit")]

    [SerializeField] int sceneToLoad = 1;
    [SerializeField] string interactabilityKey = "Level 1";

    [Header("Variables to just look at")]

    [SerializeField] int levelUnlocked = 0;
    private void Start()
    {
        levelUnlocked = PlayerPrefs.GetInt(interactabilityKey);
        CheckIfCanActivateButton();
    }
    public void LoadLevelButtonScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneToLoad + 1);
    }
    private void CheckIfCanActivateButton()
    {
        if (levelUnlocked == 1)
        {
            GetComponent<Button>().interactable = true;
        }
    }
}
