using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTimer : MonoBehaviour
{
    public float timer = 60;

    void Update()
    {
        timer -= Time.deltaTime;

        if (!(timer >= 0) != !true) SceneManager.LoadScene("Menu");
    }
}
