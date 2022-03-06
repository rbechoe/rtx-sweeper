using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private const float MOVESPEED = 5;

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.INPUT_BACK, MoveBackward);
        EventSystem.AddListener(EventType.INPUT_DOWN, MoveDown);
        EventSystem.AddListener(EventType.INPUT_FORWARD, MoveForward);
        EventSystem.AddListener(EventType.INPUT_LEFT, MoveLeft);
        EventSystem.AddListener(EventType.INPUT_RIGHT, MoveRight);
        EventSystem.AddListener(EventType.INPUT_UP, MoveUp);
        EventSystem<Vector3>.AddListener(EventType.START_POS, StartPos);

    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.INPUT_BACK, MoveBackward);
        EventSystem.RemoveListener(EventType.INPUT_DOWN, MoveDown);
        EventSystem.RemoveListener(EventType.INPUT_FORWARD, MoveForward);
        EventSystem.RemoveListener(EventType.INPUT_LEFT, MoveLeft);
        EventSystem.RemoveListener(EventType.INPUT_RIGHT, MoveRight);
        EventSystem.RemoveListener(EventType.INPUT_UP, MoveUp);
        EventSystem<Vector3>.RemoveListener(EventType.START_POS, StartPos);
    }

    private void StartPos(Vector3 position)
    {
        transform.position = position;
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
