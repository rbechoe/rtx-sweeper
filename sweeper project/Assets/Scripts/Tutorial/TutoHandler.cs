using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutoHandler : MonoBehaviour
{
    public TextMeshProUGUI lookText, moveText;
    public RawImage blackScreen;
    public PlayerController playerController; // TODO enable movement after a couple of seconds

    private void Start()
    {
        StartCoroutine(ShowLook(lookText));
        StartCoroutine(FadeInRoutine());
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    IEnumerator FadeInRoutine()
    {
        yield return new WaitForSeconds(2);

        for (int i = 0; i < 255; i++)
        {
            blackScreen.color = new Color(0, 0, 0, (255f - i) / 255f);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ShowLook(TextMeshProUGUI showText)
    {
        for (int i = 0; i < 255; i++)
        {
            showText.color = new Color(1, 1, 1, i / 255f);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(6);
        showText.enabled = false;
    }
}
