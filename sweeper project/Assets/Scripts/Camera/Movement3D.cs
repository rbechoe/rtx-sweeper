using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private GameObject rotatorObject;

    private bool inRoutine;

    private int speed = 5;
    private float moveCd = 0;
    private float moveCdReset = .25f;

    public int lowestLevel;
    public int layersAvailable;
    private int layer = 0;

    public Image level;
    public Sprite[] levels;

    private void Start()
    {
        frontSelection.transform.localScale = new Vector3(100, .9f, 100);
        midSelection.transform.localScale = new Vector3(100, .9f, 100);
        endSelection.transform.localScale = new Vector3(100, .9f, 100);
        parentSelection.transform.position = new Vector3(0, lowestLevel, 0);
        UpdateLayer();
    }

    private void OnEnable()
    {
        EventSystem.eventCollection[EventType.INPUT_FORWARD] += RotateUp;
        EventSystem.eventCollection[EventType.INPUT_BACK] += RotateDown;
        EventSystem.eventCollection[EventType.INPUT_RIGHT] += RotateRight;
        EventSystem.eventCollection[EventType.INPUT_LEFT] += RotateLeft;
        EventSystem.eventCollection[EventType.INPUT_UP] += MoveUp;
        EventSystem.eventCollection[EventType.INPUT_DOWN] += MoveDown;
        EventSystem.eventCollection[EventType.PREPARE_GAME] += RotateReset;
        EventSystem.eventCollection[EventType.RANDOM_GRID] += ResetGrid;
        EventSystem.eventCollection[EventType.ADD_LAYER] += AddLayer;
    }

    private void OnDisable()
    {
        EventSystem.eventCollection[EventType.INPUT_FORWARD] -= RotateUp;
        EventSystem.eventCollection[EventType.INPUT_BACK] -= RotateDown;
        EventSystem.eventCollection[EventType.INPUT_RIGHT] -= RotateRight;
        EventSystem.eventCollection[EventType.INPUT_LEFT] -= RotateLeft;
        EventSystem.eventCollection[EventType.INPUT_UP] -= MoveUp;
        EventSystem.eventCollection[EventType.INPUT_DOWN] -= MoveDown;
        EventSystem.eventCollection[EventType.PREPARE_GAME] -= RotateReset;
        EventSystem.eventCollection[EventType.RANDOM_GRID] -= ResetGrid;
        EventSystem.eventCollection[EventType.ADD_LAYER] -= AddLayer;
    }

    private void Update()
    {
        if (moveCd > 0)
        {
            moveCd -= Time.deltaTime;
        }
    }

    private void AddLayer()
    {
        layersAvailable++;
    }

    private void ResetGrid()
    {
        // TODO fix magical numbers
        lowestLevel = -2;
        layersAvailable = 5;
        layer = 0;
        parentSelection.transform.position = new Vector3(0, lowestLevel, 0);
    }

    public void RotateUp()
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineRotation(new Vector3(speed, 0, 0)));
    }

    public void RotateDown()
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineRotation(new Vector3(-speed, 0, 0)));
    }

    public void RotateLeft()
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineRotation(new Vector3(0, 0, speed)));
    }

    public void RotateRight()
    {
        if (inRoutine)
        {
            return;
        }
        StartCoroutine(RoutineRotation(new Vector3(0, 0, -speed)));
    }

    private void RotateReset()
    {
        rotatorObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        rotatorObject.transform.position = Vector3.zero;
        parentObj.transform.parent = rotatorObject.transform;
        parentObj.transform.localEulerAngles = Vector3.zero;
        parentObj.transform.parent = null;
        parentObj.transform.localScale = Vector3.one;
    }

    private IEnumerator RoutineRotation(Vector3 _speed)
    {
        inRoutine = true;

        rotatorObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        rotatorObject.transform.position = Vector3.zero;
        parentObj.transform.parent = rotatorObject.transform;

        for (int i = 0; i < 90 / speed; i++)
        {
            rotatorObject.transform.rotation *= Quaternion.Euler(_speed);
            yield return new WaitForEndOfFrame();
        }
        FloorAxis();
        yield return new WaitForEndOfFrame();

        parentObj.transform.parent = null;
        parentObj.transform.localScale = Vector3.one;   
        inRoutine = false;
    }

    private void FloorAxis()
    {
        rotatorObject.transform.eulerAngles = new Vector3(Mathf.RoundToInt(rotatorObject.transform.eulerAngles.x),
                                                      Mathf.RoundToInt(rotatorObject.transform.eulerAngles.y),
                                                      Mathf.RoundToInt(rotatorObject.transform.eulerAngles.z));
    }

    public void MoveUp()
    {
        if (parentSelection.transform.position.y >= lowestLevel + layersAvailable - 1) return;

        if (moveCd <= 0)
        {
            parentSelection.transform.position += Vector3.up;
            moveCd = moveCdReset;
            layer++;
            UpdateLayer();
        }
    }

    public void MoveDown()
    {
        if (parentSelection.transform.position.y <= lowestLevel) return;

        if (moveCd <= 0)
        {
            parentSelection.transform.position += Vector3.down;
            moveCd = moveCdReset;
            layer--;
            UpdateLayer();
        }
    }

    private void UpdateLayer()
    {
        level.sprite = levels[layer+1]; // 0 = default
    }
}
