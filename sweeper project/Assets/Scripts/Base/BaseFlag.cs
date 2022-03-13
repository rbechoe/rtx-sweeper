using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFlag : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            EventSystem<GameObject>.InvokeEvent(EventType.REMOVE_FLAG, gameObject);
        }
    }
}
