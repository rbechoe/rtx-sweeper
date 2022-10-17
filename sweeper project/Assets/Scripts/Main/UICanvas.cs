using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    public void PreventTileClicks()
    {
        EventSystem.InvokeEvent(EventType.IN_SETTINGS);
    }

    public void EnableTileClicks()
    {
        EventSystem.InvokeEvent(EventType.OUT_SETTINGS);
    }
}
