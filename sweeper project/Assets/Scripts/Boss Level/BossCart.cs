using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossCart : MonoBehaviour
{
    float pathPosition = 0;
    float speed = 0.5f;
    bool moving;

    CinemachineTrackedDolly trackedDolly;

    public CameraManager cameraManager;

    private void Start()
    {
        trackedDolly = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Update()
    {
        if (moving && pathPosition < 12)
        {
            pathPosition += Time.deltaTime * speed;
        }
        else if (pathPosition >= 12)
        {
            cameraManager.SetMoveableCam();
            Destroy(this);
        }
        trackedDolly.m_PathPosition = pathPosition;
    }

    public void Moving()
    {
        moving = true;
    }
}
