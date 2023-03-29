using UnityEngine;

public abstract class BaseFlag : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            EventSystem.eventCollectionParam[EventType.REMOVE_FLAG](gameObject);
        }
    }
}
