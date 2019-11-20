using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager1 : MonoBehaviour
{
    public string MainMenu;
    public string DescriptionScene;
    public string PlayScene;
    
    public static SceneManager1 Instance = null;

    private void Awake()
    {
        //Check if only one in scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //already one in scene
            Destroy(gameObject);
        }
    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene(MainMenu);
        //FindObjectOfType<AudioManager>().ResetMusic();
    }

    public void GoPlayScene()
    {
        SceneManager.LoadScene(PlayScene);
    }

    public void GoDescriptionScene()
    {
        SceneManager.LoadScene(DescriptionScene);
    }

    public void QuitGame()
    {
        PlayerPrefs.SetInt("High Score", 0);
        Application.Quit();
    }
}
