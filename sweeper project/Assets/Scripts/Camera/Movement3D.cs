using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement3D : MonoBehaviour
{
    [SerializeField]
    private GameObject parentObj;
    [SerializeField]
    private GameObject frontSelection;
    [SerializeField]
    private GameObject midSelection;
    [SerializeField]
    private GameObject endSelection;
    [SerializeField]
    private GameObject parentSelection;

    private bool inRoutine;

    private int speed = 5;
    private float moveCd = 0;
    private float moveCdReset = .25f;

    private void Start()
    {
        frontSelection.transform.localScale = new Vector3(100, .9f, 100);
        midSelection.transform.localScale = new Vector3(100, .9f, 100);
        endSelection.transform.localScale = new Vector3(100, .9f, 100);
        parentSelection.transform.position = new Vector3(0, .5f, 0);
    }

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.INPUT_FORWARD, RotateUp);
        EventSystem.AddListener(EventType.INPUT_BACK, RotateDown);
        EventSystem.AddListener(EventType.INPUT_RIGHT, RotateRight);
        EventSystem.AddListener(EventType.INPUT_LEFT, RotateLeft);
        EventSystem.AddListener(EventType.INPUT_UP, MoveUp);
        EventSystem.AddListener(EventType.INPUT_DOWN, MoveDown);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.INPUT_FORWARD, RotateUp);
        EventSystem.RemoveListener(EventType.INPUT_BACK, RotateDown);
        EventSystem.RemoveListener(EventType.INPUT_RIGHT, RotateRight);
        EventSystem.RemoveListener(EventType.INPUT_LEFT, RotateLeft);
        EventSystem.RemoveListener(EventType.INPUT_UP, MoveUp);
        EventSystem.RemoveListener(EventType.INPUT_DOWN, MoveDown);
    }

    private void Update()
    {
        if (moveCd > 0)
        {
            moveCd -= Time.deltaTime;
        }
    }

    private void RotateUp()
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineRotation(new Vector3(speed, 0, 0)));
    }

    private void RotateDown()
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineRotation(new Vector3(-speed, 0, 0)));
    }

    private void RotateLeft()
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineRotation(new Vector3(0, -speed, 0)));
    }

    private void RotateRight()
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineRotation(new Vector3(0, speed, 0)));
    }

    private IEnumerator RoutineRotation(Vector3 _speed)
    {
        inRoutine = true;
        for (int i = 0; i < 90 / speed; i++)
        {
            parentObj.transform.rotation *= Quaternion.Euler(_speed);
            yield return new WaitForEndOfFrame();
        }
        inRoutine = false;
        FloorAxis();
        yield return new WaitForEndOfFrame();
    }

    private void FloorAxis()
    {
        parentObj.transform.eulerAngles = new Vector3(Mathf.RoundToInt(parentObj.transform.eulerAngles.x),
                                                      Mathf.RoundToInt(parentObj.transform.eulerAngles.y),
                                                      Mathf.RoundToInt(parentObj.transform.eulerAngles.z));
    }

    private void MoveUp()
    {
        if (moveCd <= 0)
        {
            parentSelection.transform.position += Vector3.up;
            moveCd = moveCdReset;
        }
    }

    private void MoveDown()
    {
        if (moveCd <= 0)
        {
            parentSelection.transform.position += Vector3.down;
            moveCd = moveCdReset;
        }
    }
}