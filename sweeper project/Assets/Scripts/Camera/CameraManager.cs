using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Tiles2D;

public class CameraManager : MonoBehaviour
{
    private CinemachineTrackedDolly trackedDolly;
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private CinemachineSmoothPath startToMid;
    [SerializeField] private CinemachineSmoothPath startToLeft;
    [SerializeField] private CinemachineSmoothPath startToRight;
    [SerializeField] private GridManager midManager;
    [SerializeField] private GridManager leftManager;
    [SerializeField] private GridManager rightManager;

    private float animationTime = 0;
    private float animationEndTime = 0;
    [Tooltip("Higher is slower")]
    public float speed = 1f; 

    private void Start()
    {
        trackedDolly = virtualCam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Update()
    {
        if (animationTime < animationEndTime)
        {
            animationTime += Time.deltaTime / speed;
            trackedDolly.m_PathPosition = animationTime;
        }
    }
    
    public void StartToMidAnimation()
    {
        DisableManagers();
        midManager.enabled = true;
        midManager.gameObject.SetActive(true);
        trackedDolly.m_Path = startToMid;
        animationTime = 0;
        animationEndTime = startToMid.PathLength;
    }
    
    public void StartToRightAnimation()
    {
        DisableManagers();
        rightManager.enabled = true;
        rightManager.gameObject.SetActive(true);
        trackedDolly.m_Path = startToRight;
        animationTime = 0;
        animationEndTime = startToRight.PathLength;
    }
    
    public void StartToLeftAnimation()
    {
        DisableManagers();
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
        rightManager.enabled = false;
        midManager.gameObject.SetActive(false);
        leftManager.gameObject.SetActive(false);
        rightManager.gameObject.SetActive(false);
    }
}