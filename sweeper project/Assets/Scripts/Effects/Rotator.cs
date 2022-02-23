using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    public bool axisX, axisY, axisZ;
    public float speed;

    private float x, y, z;

    private void Start()
    {
        x = transform.localEulerAngles.x;
        y = transform.localEulerAngles.y;
        z = transform.localEulerAngles.z;
    }

    void Update()
    {
        if (axisX) x += speed * Time.deltaTime;
        if (axisY) y += speed * Time.deltaTime;
        if (axisZ) z += speed * Time.deltaTime;

        transform.localEulerAngles = new Vector3(x, y, z);
    }
}
