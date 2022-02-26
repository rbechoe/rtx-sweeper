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

    private void Start()
    {
        trackedDolly = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Update()
    {
        if (moving && pathPosition < 12) pathPosition += Time.deltaTime * speed;
        trackedDolly.m_PathPosition = pathPosition;
    }

    public void Moving()
    {
        moving = true;
    }
}
