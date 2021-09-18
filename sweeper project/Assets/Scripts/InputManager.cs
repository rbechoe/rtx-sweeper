using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Base
{
    public Dictionary<KeyCode, EventType> keybindings = new Dictionary<KeyCode, EventType>();

    protected override void Start()
    {
        keybindings.Add(KeyCode.A,           EventType.INPUT_LEFT);
        keybindings.Add(KeyCode.D,           EventType.INPUT_RIGHT);
        keybindings.Add(KeyCode.W,           EventType.INPUT_FORWARD);
        keybindings.Add(KeyCode.S,           EventType.INPUT_BACK);
        keybindings.Add(KeyCode.Space,       EventType.INPUT_UP);
        keybindings.Add(KeyCode.LeftControl, EventType.INPUT_DOWN);
    }

    protected override void Update()
    {
        // check if any key is pressed and fire event related to it
        foreach(KeyValuePair<KeyCode, EventType> keybinding in keybindings)
        {
            if (Input.GetKey(keybinding.Key))
            {
                EventSystem<Parameters>.InvokeEvent(keybinding.Value, new Parameters());
            }
        }
    }
}