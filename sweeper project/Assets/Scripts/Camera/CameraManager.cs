using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : Base
{
    private CinemachineTrackedDolly trackedDolly;
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private CinemachineSmoothPath startToMid;
    [SerializeField] private CinemachineSmoothPath startToLeft;
    [SerializeField] private GridManager2D midManager;
    [SerializeField] private GridManager2D leftManager;

    private float animationTime = 0;
    private float animationEndTime = 0;
    private float speed = 1f; // higher is slower

    protected override void Start()
    {
        trackedDolly = virtualCam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    protected override void Update()
    {
        if (animationTime < animationEndTime)
        {
            animationTime += Time.deltaTime / speed;
            trackedDolly.m_PathPosition = animationTime;
        }
    }

    // go to 13x14 grid - 1 layer
    public void StartToMidAnimation()
    {
        DisableManagers();
        EventSystem<Parameters>.InvokeEvent(EventType.RANDOM_GRID, new Parameters());
        midManager.enabled = true;
        midManager.gameObject.SetActive(true);
        trackedDolly.m_Path = startToMid;
        animationTime = 0;
        animationEndTime = startToMid.PathLength;
    }

    // go to 10x9 grid - 1 layer
    public void StartToLeftAnimation()
    {
        DisableManagers();
        EventSystem<Parameters>.InvokeEvent(EventType.RANDOM_GRID, new Parameters());
        leftManager.enabled = true;
        leftManager.gameObject.SetActive(true);
        trackedDolly.m_Path = startToLeft;
        animationTime = 0;
        animationEndTime = startToLeft.PathLength;
    }

    private void DisableManagers()
    {
        midManager.enabled = false;
        leftManager.enabled = false;
        midManager.gameObject.SetActive(false);
        leftManager.gameObject.SetActive(false);
    }
}