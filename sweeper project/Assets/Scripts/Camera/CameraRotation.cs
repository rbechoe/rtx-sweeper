using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    private Vector3 mousePos, mouseStart, mouseDirection;

    private float xRotation, yRotation;

    private float speedSlow = 5f;
    private float speedMultiplier = 50f;

    void Update()
    {
        #region Mouse Input
        if (Input.GetMouseButton(2) || Input.GetMouseButtonDown(2) || Input.GetMouseButtonUp(2))
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
                mouseDirection = (mouseStart - mousePos) / speedSlow;
            }

            if (Input.GetMouseButtonUp(2))
            {
                // get start pos x y
                mouseDirection = Vector2.zero;
            }

            xRotation += mouseDirection.y * Time.deltaTime;
            yRotation += mouseDirection.x * Time.deltaTime;
        }
        #endregion

        #region Keyboard Input
        float xRot = 0;
        float yRot = 0;
        if (Input.GetKey(KeyCode.S))
        {
            xRot = 1;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            xRot = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            yRot = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            yRot = -1;
        }
        xRotation += xRot * speedMultiplier * Time.deltaTime;
        yRotation += yRot * speedMultiplier * Time.deltaTime;
        #endregion

        transform.eulerAngles = new Vector3(-xRotation, yRotation, 0);
    }
}
