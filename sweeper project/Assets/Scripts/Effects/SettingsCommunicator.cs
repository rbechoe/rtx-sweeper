using UnityEngine;

public class SettingsCommunicator : MonoBehaviour
{
    public void PlayClick()
    {
        EventSystem.eventCollection[EventType.PLAY_CLICK_SFX]();
    }
}
