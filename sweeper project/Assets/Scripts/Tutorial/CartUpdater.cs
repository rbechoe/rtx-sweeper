using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CartUpdater : MonoBehaviour
{
    public float speed = 0.5f;
    public float cartPos;
    public List<int> checkPoints;

    CinemachineVirtualCamera vCam;
    CinemachineTrackedDolly dolly;

    public RawImage blackScreen;

    bool started;

    float timer = 2;

    void Start()
    {
        vCam = gameObject.GetComponent<CinemachineVirtualCamera>();
        dolly = vCam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            blackScreen.color = new Color(0.1f, 0.1f, 0.1f, timer / 2f);
        }

        if (timer <= 0)
        {
            blackScreen.gameObject.SetActive(false);
            started = true;
        }

        if (started)
        {
            if ((checkPoints.Count > 0 && cartPos < checkPoints[0]) || checkPoints.Count == 0)
            {
                cartPos += Time.deltaTime * speed;
            }
            dolly.m_PathPosition = cartPos;
        }
    }
}
