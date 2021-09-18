using UnityEngine;

public class Flag : Base
{
    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.END_GAME, ReSize);
    }

    private void OnDisable()
    {
        EventSystem<Parameters>.RemoveListener(EventType.END_GAME, ReSize);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Parameters param = new Parameters();
            param.gameObjects.Add(gameObject);
            EventSystem<Parameters>.InvokeEvent(EventType.REMOVE_FLAG, param);
        }
    }

    private void ReSize(object value)
    {
        transform.localScale = new Vector3(0.5f, transform.localScale.y, 0.5f);
    }
}