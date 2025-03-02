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
    [SerializeField] private BaseGridManager midManager;
    [SerializeField] private BaseGridManager leftManager;
    [SerializeField] private BaseGridManager rightManager;

    public float animationTime = 0;
    public float animationEndTime = 0;
    [Tooltip("Higher is slower")]
    public bool disableAnimations;
    public float speed = 1f;
    
    private Transform emptyParent;
    private Transform baseTransform;
    private float currentSpeed = 5f;
    private bool shiftPressed;
    private float xCount;
    private float zCount;

    [Header("Movement Settings")]
    public float baseSpeed = 5f;
    public float fastSpeed = 10f;
    public int relativeXRange = 10;
    public int minHeight = 10;
    public int maxHeight = 40;
    public int relativeZRange = 10;

    private void OnEnable()
    {
        EventSystem.eventCollection[EventType.INPUT_FORWARD] += MoveForward;
        EventSystem.eventCollection[EventType.INPUT_BACK] += MoveBack;
        EventSystem.eventCollection[EventType.INPUT_LEFT] += MoveLeft;
        EventSystem.eventCollection[EventType.INPUT_RIGHT] += MoveRight;
        EventSystem.eventCollection[EventType.INPUT_UP] += MoveUp;
        EventSystem.eventCollection[EventType.INPUT_DOWN] += MoveDown;
        EventSystem.eventCollection[EventType.INPUT_SCROLL_DOWN] += ScrollDown;
        EventSystem.eventCollection[EventType.INPUT_SCROLL_UP] += ScrollUp;
        EventSystem.eventCollection[EventType.INPUT_SPEED] += FastSpeed;
    }

    private void OnDisable()
    {
        EventSystem.eventCollection[EventType.INPUT_FORWARD] -= MoveForward;
        EventSystem.eventCollection[EventType.INPUT_BACK] -= MoveBack;
        EventSystem.eventCollection[EventType.INPUT_LEFT] -= MoveLeft;
        EventSystem.eventCollection[EventType.INPUT_RIGHT] -= MoveRight;
        EventSystem.eventCollection[EventType.INPUT_UP] -= MoveUp;
        EventSystem.eventCollection[EventType.INPUT_DOWN] -= MoveDown;
        EventSystem.eventCollection[EventType.INPUT_SCROLL_DOWN] -= ScrollDown;
        EventSystem.eventCollection[EventType.INPUT_SCROLL_UP] -= ScrollUp;
        EventSystem.eventCollection[EventType.INPUT_SPEED] -= FastSpeed;
    }

    private void Start()
    {
        trackedDolly = virtualCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Update()
    {
        if (!disableAnimations)
        {
            if (animationTime < animationEndTime)
            {
                animationTime += Time.deltaTime / speed;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    animationTime = animationEndTime - 1;
                }

                trackedDolly.m_PathPosition = animationTime;
            }

            if (animationEndTime != 0 && animationTime >= animationEndTime && virtualCam.enabled)
            {
                SetMoveableCam();
            }
        }

        currentSpeed = (shiftPressed) ? fastSpeed : baseSpeed;
        if (shiftPressed) shiftPressed = false;
    }

    public void SetMoveableCam()
    {
        virtualCam.enabled = false;
        if (this.emptyParent != null)
        {
            camera.transform.parent = null;
            Destroy(this.emptyParent.gameObject);
        }

        GameObject emptyParent = new GameObject();
        emptyParent.transform.eulerAngles = new Vector3(0, camera.transform.eulerAngles.y, 0);
        emptyParent.transform.position = camera.transform.position;
        camera.transform.parent = emptyParent.transform;
        baseTransform = emptyParent.transform;
        xCount = 0;
        zCount = 0;
        this.emptyParent = emptyParent.transform;
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

    public void ResetToStart()
    {
        animationTime = 0;
        animationEndTime = 0;
        trackedDolly.m_PathPosition = 0;
        virtualCam.enabled = true;
    }

    private void SetTrack(CinemachineSmoothPath path)
    {
        trackedDolly.m_Path = path;
        animationTime = 0;
        animationEndTime = path.m_Waypoints.Length;
    }

    private void DisableManagers()
    {
        if (midManager != null) midManager.enabled = false;
        if (leftManager != null) leftManager.enabled = false;
        if (rightManager != null) rightManager.enabled = false;
        if (midManager != null) midManager.gameObject.SetActive(false);
        if (leftManager != null) leftManager.gameObject.SetActive(false);
        if (rightManager != null) rightManager.gameObject.SetActive(false);
    }

    private void FastSpeed()
    {
        shiftPressed = true;
    }

    private void MoveForward()
    {
        if (emptyParent == null) return;

        if (zCount < relativeZRange)
        {
            zCount += Time.deltaTime * currentSpeed;
            emptyParent.position += emptyParent.forward * Time.deltaTime * currentSpeed;
        }
    }

    private void MoveBack()
    {
        if (emptyParent == null) return;

        if (zCount > -relativeZRange)
        {
            zCount -= Time.deltaTime * currentSpeed;
            emptyParent.position -= emptyParent.forward * Time.deltaTime * currentSpeed;
        }
    }

    private void MoveLeft()
    {
        if (emptyParent == null) return;

        if (xCount > -relativeXRange)
        {
            xCount -= Time.deltaTime * currentSpeed;
            emptyParent.position -= emptyParent.right * Time.deltaTime * currentSpeed;
        }
    }

    private void MoveRight()
    {
        if (emptyParent == null) return;

        if (xCount < relativeXRange)
        {
            xCount += Time.deltaTime * currentSpeed;
            emptyParent.position += emptyParent.right * Time.deltaTime * currentSpeed;
        }
    }

    private void MoveUp()
    {
        if (emptyParent == null) return;

        if (emptyParent.position.y <= maxHeight)
            emptyParent.position += Vector3.up * Time.deltaTime * currentSpeed;
    }

    private void MoveDown()
    {
        if (emptyParent == null) return;

        if (emptyParent.position.y >= minHeight)
            emptyParent.position += Vector3.down * Time.deltaTime * currentSpeed;
    }

    private void ScrollUp()
    {
        if (emptyParent == null) return;

        if (emptyParent.position.y <= maxHeight)
            emptyParent.position += Vector3.up * Time.deltaTime * (currentSpeed * currentSpeed);
    }

    private void ScrollDown()
    {
        if (emptyParent == null) return;

        if (emptyParent.position.y >= minHeight)
            emptyParent.position += Vector3.down * Time.deltaTime * (currentSpeed * currentSpeed);
    }
}