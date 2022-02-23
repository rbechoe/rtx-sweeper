using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CartUpdater : MonoBehaviour
{
    public float speed = 0.5f;
    public float cartPos;
    public List<int> checkPoints;

    CinemachineVirtualCamera vCam;
    CinemachineTrackedDolly dolly;

    void Start()
    {
        vCam = gameObject.GetComponent<CinemachineVirtualCamera>();
        dolly = vCam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    void Update()
    {
        if ((checkPoints.Count > 0 && cartPos < checkPoints[0]) || checkPoints.Count == 0)
        {
            cartPos += Time.deltaTime * speed;
        }
        dolly.m_PathPosition = cartPos;
    }
}
