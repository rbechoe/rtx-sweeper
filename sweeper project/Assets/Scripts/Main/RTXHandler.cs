using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTXHandler : MonoBehaviour
{
    public List<GameObject> rtxObjects = new List<GameObject>();
    public List<GameObject> defaultObjects = new List<GameObject>();

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.ENABLE_RTX, EnableRTX);
        EventSystem.AddListener(EventType.DISABLE_RTX, DisableRTX);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.ENABLE_RTX, EnableRTX);
        EventSystem.RemoveListener(EventType.DISABLE_RTX, DisableRTX);
    }

    private void EnableRTX()
    {
        foreach (GameObject obj in rtxObjects)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in defaultObjects)
        {
            obj.SetActive(false);
        }
    }

    private void DisableRTX()
    {
        foreach (GameObject obj in rtxObjects)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in defaultObjects)
        {
            obj.SetActive(true);
        }
    }
}
