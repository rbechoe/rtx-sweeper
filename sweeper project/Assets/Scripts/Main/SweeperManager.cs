using UnityEngine;
using UnityEngine.SceneManagement;

public class SweeperManager : MonoBehaviour
{
    public void StartGame()
    {
        EventSystem.eventCollection[EventType.RANDOM_GRID]();
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
