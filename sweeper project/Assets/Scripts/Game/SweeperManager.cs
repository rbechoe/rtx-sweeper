using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SweeperManager : Base
{
    // TODO move to global settings manager or something
    protected override void Start()
    {
        base.Start();

        if (!Screen.fullScreen)
        {
            Screen.SetResolution(1600, 900, false);
        }
        else
        {
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
    }

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.F12))
        {
            if (!Screen.fullScreen)
            {
                Screen.SetResolution(Screen.width, Screen.height, true);
            }
            else
            {
                Screen.SetResolution(1600, 900, false);
            }
        }
    }

    public void StartGame()
    {
        EventSystem.InvokeEvent(EventType.RANDOM_GRID);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
