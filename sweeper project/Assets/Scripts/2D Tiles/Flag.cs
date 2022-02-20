using UnityEngine;

public class Flag : Base
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            EventSystem<GameObject>.InvokeEvent(EventType.REMOVE_FLAG, gameObject);
        }
    }
}
