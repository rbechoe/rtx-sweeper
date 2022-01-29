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
    public PlayerController playerController;

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

    IEnumerator ShowLook(TextMeshProUGUI showText, float waitTime = 0)
    {
        yield return new WaitForSeconds(waitTime);

        for (int i = 0; i < 255; i++)
        {
            lookText.color = new Color(1, 1, 1, i / 255f);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(4);
        lookText.enabled = false;
        playerController.ActivateMovement();

        for (int i = 0; i < 255; i++)
        {
            moveText.color = new Color(1, 1, 1, i / 255f);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(4);
        moveText.enabled = false;
    }
}
