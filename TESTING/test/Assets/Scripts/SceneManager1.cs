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
        //should be able to go to playScreen
        //used in NealStartScreen->PlayButton
        SceneManager.LoadScene(PlayScene);
    }

    public void ResumeGame()
    {
        //should be able to go back to playscreen and resume game
        ////used in NealMenu->ResumeButton
    }

    public void GoDescriptionScene()
    {
        SceneManager.LoadScene(DescriptionScene);
    }

    public void GoOptions()
    {
        //should be able to go to options
        //used in NealStartScreen->OptionButton
    }

    public void QuitGame()
    {
        //should be able to quit the game
        //used in NealStartScreen->PlayButton
        Application.Quit();
    }
}
