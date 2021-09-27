using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SweeperManager : Base
{
    public void StartGame()
    {
        EventSystem<Parameters>.InvokeEvent(EventType.RANDOM_GRID, new Parameters());
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