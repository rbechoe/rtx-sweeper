using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private GameObject camera;
    private CinemachineTrackedDolly trackedDolly;
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private CinemachineSmoothPath startToMid;
    [SerializeField] private CinemachineSmoothPath startToLeft;
    [SerializeField] private CinemachineSmoothPath startToRight;
    [SerializeField] private GridManager2D midManager;
    [SerializeField] private GridManager2D leftManager;
    [SerializeField] private GridManager2D rightManager;

    public float animationTime = 0;
    public float animationEndTime = 0;
    [Tooltip("Higher is slower")]
    public float speed = 1f;

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.INPUT_FORWARD, MoveForward);
        EventSystem.AddListener(EventType.INPUT_BACK, MoveBack);
        EventSystem.AddListener(EventType.INPUT_LEFT, MoveLeft);
        EventSystem.AddListener(EventType.INPUT_RIGHT, MoveRight);
        EventSystem.AddListener(EventType.INPUT_UP, MoveUp);
        EventSystem.AddListener(EventType.INPUT_DOWN, MoveDown);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.INPUT_FORWARD, MoveForward);
        EventSystem.RemoveListener(EventType.INPUT_BACK, MoveBack);
        EventSystem.RemoveListener(EventType.INPUT_LEFT, MoveLeft);
        EventSystem.RemoveListener(EventType.INPUT_RIGHT, MoveRight);
        EventSystem.RemoveListener(EventType.INPUT_UP, MoveUp);
        EventSystem.RemoveListener(EventType.INPUT_DOWN, MoveDown);
    }

    private void Start()
    {
        trackedDolly = virtualCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Update()
    {
        if (animationTime < animationEndTime)
        {
            animationTime += Time.deltaTime / speed;
            trackedDolly.m_PathPosition = animationTime;
        }

        if (animationEndTime != 0 && animationTime >= animationEndTime && virtualCam.enabled)
        {
            virtualCam.enabled = false;
        }
    }
    
    public void StartToMidAnimation()
    {
        DisableManagers();
        midManager.enabled = true;
        midManager.gameObject.SetActive(true);
        SetTrack(startToMid);
    }
    
    public void StartToRightAnimation()
    {
        DisableManagers();
        rightManager.enabled = true;
        rightManager.gameObject.SetActive(true);
        SetTrack(startToRight);
    }
    
    public void StartToLeftAnimation()
    {
        DisableManagers();
        leftManager.enabled = true;
        leftManager.gameObject.SetActive(true);
        SetTrack(startToLeft);
    } 

    private void SetTrack(CinemachineSmoothPath path)
    {
        trackedDolly.m_Path = path;
        animationTime = 0;
        animationEndTime = path.m_Waypoints.Length;
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

    private void MoveForward()
    {
        camera.transform.position += camera.transform.forward * Time.deltaTime * 5f;
    }

    private void MoveBack()
    {
        camera.transform.position -= camera.transform.forward * Time.deltaTime * 5f;
    }

    private void MoveLeft()
    {
        camera.transform.position -= camera.transform.right * Time.deltaTime * 5f;
    }

    private void MoveRight()
    {
        camera.transform.position += camera.transform.right * Time.deltaTime * 5f;
    }

    private void MoveUp()
    {
        camera.transform.position += Vector3.up * Time.deltaTime * 5f;
    }

    private void MoveDown()
    {
        camera.transform.position += Vector3.down * Time.deltaTime * 5f;
    }
}