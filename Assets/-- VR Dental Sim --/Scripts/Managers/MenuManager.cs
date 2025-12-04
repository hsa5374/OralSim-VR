using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject levelSelectionMenu = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void NewUser()
    {
        PlayerPrefs.SetInt("Level", 1);
        levelSelectionMenu.SetActive(true);
    }


    public void QuitApp()
    {
        Application.Quit();
    }



    public void SelectLevel(int level)
    {
        PlayerPrefs.SetInt("Level", level);

        switch (level)
        {
            case 1:
                SceneManager.LoadScene("Level1");
                return;
            case 2:
                SceneManager.LoadScene("Level2");
                return;
            case 3:
                SceneManager.LoadScene("Level3");
                return;
            default:
                SceneManager.LoadScene("Level1");
                return;
        }
    }

}
