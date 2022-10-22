using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEffect : MonoBehaviour
{
    public AudioSource audioSource;

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.PLAY_CLICK_SFX, PlayClick);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.PLAY_CLICK_SFX, PlayClick);
    }

    public void PlayClick()
    {
        audioSource.Play();
    }
}
