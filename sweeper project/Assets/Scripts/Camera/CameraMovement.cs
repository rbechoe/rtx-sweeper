using UnityEngine;

public class CameraMovement : Base
{
    private const float MOVESPEED = 5;

    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.INPUT_BACK, MoveBackward);
        EventSystem<Parameters>.AddListener(EventType.INPUT_DOWN, MoveDown);
        EventSystem<Parameters>.AddListener(EventType.INPUT_FORWARD, MoveForward);
        EventSystem<Parameters>.AddListener(EventType.INPUT_LEFT, MoveLeft);
        EventSystem<Parameters>.AddListener(EventType.INPUT_RIGHT, MoveRight);
        EventSystem<Parameters>.AddListener(EventType.INPUT_UP, MoveUp);
        EventSystem<Parameters>.AddListener(EventType.START_POS, StartPos);

    }

    private void OnDisable()
    {
        EventSystem<Parameters>.RemoveListener(EventType.INPUT_BACK, MoveBackward);
        EventSystem<Parameters>.RemoveListener(EventType.INPUT_DOWN, MoveDown);
        EventSystem<Parameters>.RemoveListener(EventType.INPUT_FORWARD, MoveForward);
        EventSystem<Parameters>.RemoveListener(EventType.INPUT_LEFT, MoveLeft);
        EventSystem<Parameters>.RemoveListener(EventType.INPUT_RIGHT, MoveRight);
        EventSystem<Parameters>.RemoveListener(EventType.INPUT_UP, MoveUp);
        EventSystem<Parameters>.RemoveListener(EventType.START_POS, StartPos);
    }

    private void StartPos(Parameters param)
    {
        transform.position = param.vector3s[0];
    }

    private void MoveLeft(object value)
    {
        transform.Translate(Vector3.left * Time.deltaTime * MOVESPEED);
    }

    private void MoveRight(object value)
    {
        transform.Translate(Vector3.right * Time.deltaTime * MOVESPEED);
    }

    private void MoveForward(object value)
    {
        transform.Translate(Vector3.forward * Time.deltaTime * MOVESPEED);
    }

    private void MoveBackward(object value)
    {
        transform.Translate(Vector3.back * Time.deltaTime * MOVESPEED);
    }

    private void MoveUp(object value)
    {
        transform.Translate(Vector3.up * Time.deltaTime * MOVESPEED);
    }

    private void MoveDown(object value)
    {
        transform.Translate(Vector3.down * Time.deltaTime * MOVESPEED);
    }
}