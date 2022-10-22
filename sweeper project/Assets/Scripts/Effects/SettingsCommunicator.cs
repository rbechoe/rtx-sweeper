using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsCommunicator : MonoBehaviour
{
    public void PlayClick()
    {
        EventSystem.InvokeEvent(EventType.PLAY_CLICK_SFX);
    }
}
