using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField]
    private Vector3 mousePos, mouseStart, mouseDirection;

    private float xAxis, yAxis;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            // get start pos x y
            mouseStart = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            // update camera orientation
            mousePos = Input.mousePosition;
            mouseDirection = (mouseStart - mousePos) / 10f;
        }
        if (Input.GetMouseButtonUp(2))
        {
            // get start pos x y
            mouseDirection = Vector2.zero;
        }
        xAxis += mouseDirection.y * Time.deltaTime;
        yAxis += mouseDirection.x * Time.deltaTime;

        transform.eulerAngles = new Vector3(xAxis, yAxis, 0);
    }
}
