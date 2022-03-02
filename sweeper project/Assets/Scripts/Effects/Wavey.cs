using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wavey : MonoBehaviour
{
    public float distance = 1f;
    public float speed = 1f;
    private float timeOffset = 0; // makes it a bit random

    public bool axisX, axisY, axisZ;

    private Vector3 startEuler;

    private void Start()
    {
        startEuler = transform.localEulerAngles;
        timeOffset = Random.Range(0, 1000);
    }

    private void Update()
    {
        float val = Mathf.Sin(Time.time * speed + timeOffset) * distance;
        float valX = (axisX) ? val : 0;
        float valY = (axisY) ? val : 0;
        float valZ = (axisZ) ? val : 0;
        transform.localEulerAngles = startEuler + new Vector3(valX, valY, valZ);
    }
}
