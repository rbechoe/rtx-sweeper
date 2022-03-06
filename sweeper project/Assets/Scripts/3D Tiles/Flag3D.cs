using UnityEngine;

public class Flag3D : MonoBehaviour
{
    private bool hovered;

    private void Update()
    {
        if (Input.GetMouseButtonUp(1) && hovered)
        {
            EventSystem<GameObject>.InvokeEvent(EventType.REMOVE_FLAG, gameObject);
        }
    }

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.END_GAME, ReSize);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.END_GAME, ReSize);
    }

    private void ReSize()
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
