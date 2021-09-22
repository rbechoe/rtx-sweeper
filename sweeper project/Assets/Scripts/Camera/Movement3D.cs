using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement3D : Base
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

    protected override void Start()
    {
        base.Start();
        frontSelection.transform.localScale = new Vector3(100, .9f, 100);
        midSelection.transform.localScale = new Vector3(100, .9f, 100);
        endSelection.transform.localScale = new Vector3(100, .9f, 100);
        parentSelection.transform.position = new Vector3(0, .5f, 0);
    }

    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.INPUT_FORWARD, RotateUp);
        EventSystem<Parameters>.AddListener(EventType.INPUT_BACK, RotateDown);
        EventSystem<Parameters>.AddListener(EventType.INPUT_RIGHT, RotateRight);
        EventSystem<Parameters>.AddListener(EventType.INPUT_LEFT, RotateLeft);
    }

    private void OnDisable()
    {
        EventSystem<Parameters>.RemoveListener(EventType.INPUT_FORWARD, RotateUp);
        EventSystem<Parameters>.RemoveListener(EventType.INPUT_BACK, RotateDown);
        EventSystem<Parameters>.RemoveListener(EventType.INPUT_RIGHT, RotateRight);
        EventSystem<Parameters>.RemoveListener(EventType.INPUT_LEFT, RotateLeft);
    }

    private void RotateUp(object value)
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineUp());
    }

    private IEnumerator RoutineUp()
    {
        inRoutine = true;
        for (int i = 0; i < 90; i++)
        {
            parentObj.transform.rotation *= Quaternion.Euler(1f, 0, 0);
            yield return new WaitForEndOfFrame();
        }
        inRoutine = false;
        FloorAxis();
        yield return new WaitForEndOfFrame();
    }

    private void RotateDown(object value)
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineDown());
    }

    private IEnumerator RoutineDown()
    {
        inRoutine = true;
        for (int i = 0; i < 90; i++)
        {
            parentObj.transform.rotation *= Quaternion.Euler(-1f, 0, 0);
            yield return new WaitForEndOfFrame();
        }
        inRoutine = false;
        FloorAxis();
        yield return new WaitForEndOfFrame();
    }

    private void RotateLeft(object value)
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineLeft());
    }

    private IEnumerator RoutineLeft()
    {
        inRoutine = true;
        for (int i = 0; i < 90; i++)
        {
            parentObj.transform.rotation *= Quaternion.Euler(0, -1f, 0);
            yield return new WaitForEndOfFrame();
        }
        inRoutine = false;
        FloorAxis();
        yield return new WaitForEndOfFrame();
    }

    private void RotateRight(object value)
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineRight());
    }

    private IEnumerator RoutineRight()
    {
        inRoutine = true;
        for (int i = 0; i < 90; i++)
        {
            parentObj.transform.rotation *= Quaternion.Euler(0, 1f, 0);
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
}