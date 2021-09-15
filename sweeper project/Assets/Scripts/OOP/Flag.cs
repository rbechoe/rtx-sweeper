using UnityEngine;

public class Flag : Base
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            EventSystem<GameObject>.InvokeEvent(EventType.REMOVE_FLAG, gameObject);
        }
    }
}