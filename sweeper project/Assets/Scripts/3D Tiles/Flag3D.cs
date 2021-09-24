using UnityEngine;

public class Flag3D : Base
{
    private bool hovered;

    protected override void Update()
    {
        if (Input.GetMouseButtonUp(1) && hovered)
        {
            Parameters param = new Parameters();
            param.gameObjects.Add(gameObject);
            EventSystem<Parameters>.InvokeEvent(EventType.REMOVE_FLAG, param);
        }
    }

    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.END_GAME, ReSize);
    }

    private void OnDisable()
    {
        EventSystem<Parameters>.RemoveListener(EventType.END_GAME, ReSize);
    }

    private void ReSize(object value)
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Picker"))
        {
            hovered = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Picker"))
        {
            hovered = false;
        }
    }
}