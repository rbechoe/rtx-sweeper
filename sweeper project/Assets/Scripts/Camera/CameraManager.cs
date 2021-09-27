using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : Base
{
    private CinemachineTrackedDolly trackedDolly;
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private CinemachineSmoothPath startToMid;
    [SerializeField] private GridManager2D midManager;

    private float animationTime = 0;
    private float animationEndTime = 0;
    private float speed = 2f; // higher is slower

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
        DisableMangers();
        EventSystem<Parameters>.InvokeEvent(EventType.RANDOM_GRID, new Parameters());
        midManager.enabled = true;
        trackedDolly.m_Path = startToMid;
        animationTime = 0;
        animationEndTime = startToMid.PathLength;
    }

    private void DisableMangers()
    {
        midManager.enabled = false;
    }
}