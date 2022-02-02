using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEffect : MonoBehaviour
{
    public AudioSource audioSource; 

    public void PlayClick()
    {
        audioSource.Play();
    }
}
