using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : Base
{
    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.INPUT_BACK, MoveBackward);
        EventSystem<Parameters>.AddListener(EventType.INPUT_DOWN, MoveDown);
        EventSystem<Parameters>.AddListener(EventType.INPUT_FORWARD, MoveForward);
        EventSystem<Parameters>.AddListener(EventType.INPUT_LEFT, MoveLeft);
        EventSystem<Parameters>.AddListener(EventType.INPUT_RIGHT, MoveRight);
        EventSystem<Parameters>.AddListener(EventType.INPUT_UP, MoveUp);
    }

    private void OnDisable()
    {
        
    }

    private void MoveLeft(object value)
    {
        print("received MoveLeft");
    }

    private void MoveRight(object value)
    {
        print("received MoveRight");
    }

    private void MoveForward(object value)
    {
        print("received MoveForward");
    }

    private void MoveBackward(object value)
    {
        print("received MoveBackward");
    }

    private void MoveUp(object value)
    {
        print("received MoveUp");
    }

    private void MoveDown(object value)
    {
        print("received MoveDown");
    }
}