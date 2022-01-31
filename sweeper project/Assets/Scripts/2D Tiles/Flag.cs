using UnityEngine;

public class Flag : Base
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Parameters param = new Parameters();
            param.gameObjects.Add(gameObject);
            EventSystem<Parameters>.InvokeEvent(EventType.REMOVE_FLAG, param);
        }
    }
}