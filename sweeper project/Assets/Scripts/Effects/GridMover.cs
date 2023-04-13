using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMover : MonoBehaviour
{
    private const float MOVESPEED = 5;

    public Vector2 xMinMax = new Vector2(-10, 10);
    public Vector2 yMinMax = new Vector2(-10, 10);

    private void OnEnable()
    {
        EventSystem.eventCollection[EventType.INPUT_BACK] += MoveUp;
        EventSystem.eventCollection[EventType.INPUT_LEFT] += MoveRight;
        EventSystem.eventCollection[EventType.INPUT_RIGHT] += MoveLeft;
        EventSystem.eventCollection[EventType.INPUT_FORWARD] += MoveDown;

    }

    private void OnDisable()
    {
        EventSystem.eventCollection[EventType.INPUT_BACK] -= MoveUp;
        EventSystem.eventCollection[EventType.INPUT_LEFT] -= MoveRight;
        EventSystem.eventCollection[EventType.INPUT_RIGHT] -= MoveLeft;
        EventSystem.eventCollection[EventType.INPUT_FORWARD] -= MoveDown;
    }

    private void MoveLeft()
    {
        if (transform.position.x < xMinMax.x)
        {
            return;
        }
        transform.Translate(Vector3.left * Time.deltaTime * MOVESPEED);
    }

    private void MoveRight()
    {
        if (transform.position.x > xMinMax.y)
        {
            return;
        }
        transform.Translate(Vector3.right * Time.deltaTime * MOVESPEED);
    }

    private void MoveUp()
    {
        if (transform.position.y > yMinMax.y)
        {
            return;
        }
        transform.Translate(Vector3.up * Time.deltaTime * MOVESPEED);
    }

    private void MoveDown()
    {
        if (transform.position.y < yMinMax.x)
        {
            return;
        }
        transform.Translate(Vector3.down * Time.deltaTime * MOVESPEED);
    }
}
