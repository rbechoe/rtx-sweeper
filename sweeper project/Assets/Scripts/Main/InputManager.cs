using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Dictionary<KeyCode, EventType> keybindings = new Dictionary<KeyCode, EventType>();
    public Dictionary<KeyCode, EventType> keybindingsUp = new Dictionary<KeyCode, EventType>();

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.INPUT_F12, ToggleFullscreen);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.INPUT_F12, ToggleFullscreen);
    }

    private void Start()
    {
        // TODO read from txt file as settings
        keybindings.Add(KeyCode.A,           EventType.INPUT_LEFT);
        keybindings.Add(KeyCode.D,           EventType.INPUT_RIGHT);
        keybindings.Add(KeyCode.W,           EventType.INPUT_FORWARD);
        keybindings.Add(KeyCode.S,           EventType.INPUT_BACK);
        keybindings.Add(KeyCode.Space,       EventType.INPUT_UP);
        keybindings.Add(KeyCode.LeftControl, EventType.INPUT_DOWN);
        keybindingsUp.Add(KeyCode.F12,       EventType.INPUT_F12);

        if (!Screen.fullScreen)
        {
            Screen.SetResolution(1600, 900, false);
        }
        else
        {
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
    }

    private void Update()
    {
        // check if any key is pressed and fire event related to it
        foreach (KeyValuePair<KeyCode, EventType> keybinding in keybindings)
        {
            if (Input.GetKey(keybinding.Key))
            {
                EventSystem.InvokeEvent(keybinding.Value);
            }
        }

        // check if any key is released and fire event related to it
        foreach (KeyValuePair<KeyCode, EventType> keybinding in keybindingsUp)
        {
            if (Input.GetKeyUp(keybinding.Key))
            {
                EventSystem.InvokeEvent(keybinding.Value);
            }
        }
    }

    private void ToggleFullscreen()
    {
        if (!Screen.fullScreen)
        {
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
        else
        {
            Screen.SetResolution(1600, 900, false);
        }
    }
}
