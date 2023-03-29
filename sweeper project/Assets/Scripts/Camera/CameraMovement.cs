using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private const float MOVESPEED = 5;

    private void OnEnable()
    {
        EventSystem.eventCollection[EventType.INPUT_BACK] += MoveBackward;
        EventSystem.eventCollection[EventType.INPUT_DOWN] += MoveDown;
        EventSystem.eventCollection[EventType.INPUT_FORWARD] += MoveForward;
        EventSystem.eventCollection[EventType.INPUT_LEFT] += MoveLeft;
        EventSystem.eventCollection[EventType.INPUT_RIGHT] += MoveRight;
        EventSystem.eventCollection[EventType.INPUT_UP] += MoveUp;
        EventSystem.eventCollection[EventType.INPUT_SCROLL_DOWN] += MoveDown;
        EventSystem.eventCollection[EventType.INPUT_SCROLL_UP] += MoveUp;
        EventSystem.eventCollectionParam[EventType.START_POS] += StartPos;

    }

    private void OnDisable()
    {
        EventSystem.eventCollection[EventType.INPUT_BACK] -= MoveBackward;
        EventSystem.eventCollection[EventType.INPUT_DOWN] -= MoveDown;
        EventSystem.eventCollection[EventType.INPUT_FORWARD] -= MoveForward;
        EventSystem.eventCollection[EventType.INPUT_LEFT] -= MoveLeft;
        EventSystem.eventCollection[EventType.INPUT_RIGHT] -= MoveRight;
        EventSystem.eventCollection[EventType.INPUT_UP] -= MoveUp;
        EventSystem.eventCollection[EventType.INPUT_SCROLL_DOWN] -= MoveDown;
        EventSystem.eventCollection[EventType.INPUT_SCROLL_UP] -= MoveUp;
        EventSystem.eventCollectionParam[EventType.START_POS] -= StartPos;
    }

    private void StartPos(object value)
    {
        transform.position = (Vector3)value;
    }

    private void MoveLeft()
    {
        transform.Translate(Vector3.left * Time.deltaTime * MOVESPEED);
    }

    private void MoveRight()
    {
        transform.Translate(Vector3.right * Time.deltaTime * MOVESPEED);
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * MOVESPEED);
    }

    private void MoveBackward()
    {
        transform.Translate(Vector3.back * Time.deltaTime * MOVESPEED);
    }

    private void MoveUp()
    {
        transform.Translate(Vector3.up * Time.deltaTime * MOVESPEED);
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * Time.deltaTime * MOVESPEED);
    }
}
