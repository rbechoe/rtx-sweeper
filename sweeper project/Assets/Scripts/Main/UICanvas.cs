using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    public GameObject rtxButtons, rtxOffText, rtxOffBtn, rtxOnBtn;

    private void Start()
    {
        // if version is 12 then enable graphics button otherwise disable it
        string dxVersion = SystemInfo.graphicsDeviceVersion;
        if (dxVersion.Contains("Direct3D 12"))
        {
            rtxButtons.SetActive(true);
            rtxOffText.SetActive(false);
        }
        else
        {
            rtxButtons.SetActive(false);
            rtxOffText.SetActive(true);
        }

        if (Settings.Instance.GetRTX())
        {
            rtxOnBtn.SetActive(true);
            rtxOffBtn.SetActive(false);
        }
        else
        {
            rtxOnBtn.SetActive(false);
            rtxOffBtn.SetActive(true);
        }
    }

    public void PreventTileClicks()
    {
        EventSystem.eventCollection[EventType.IN_SETTINGS]();
    }

    public void EnableTileClicks()
    {
        EventSystem.eventCollection[EventType.OUT_SETTINGS]();
    }
}
