using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float vertical;
    float horizontal;
    float angleX, angleY, mouseX, mouseY;

    public GameObject playerCam;

    [Header("Movement Settings")]
    public float speed = 5;
    public float camSensitivity = 100;
    public float camClamp = 90;

    void Update()
    {
        vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(horizontal, 0, vertical);

        CamMovement();
    }

    void CamMovement()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        angleX += mouseX * Time.deltaTime * camSensitivity;
        angleY += mouseY * Time.deltaTime * camSensitivity;

        transform.rotation = Quaternion.Euler(0, angleX, 0);
        angleY = Mathf.Clamp(angleY, -camClamp, camClamp);
        playerCam.transform.localRotation = Quaternion.Euler(-angleY, 0, 0);
    }
}