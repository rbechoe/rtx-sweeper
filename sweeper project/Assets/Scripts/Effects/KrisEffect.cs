using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrisEffect : MonoBehaviour
{
    public GameObject krisObject;

    private bool show, shouldWait, shouldShow;
    private float waitTime, showTime;

    private void Awake()
    {
        krisObject.SetActive(false);
    }

    private void Start()
    {
        show = (Random.Range(0, 10) > 2) ? true : false;
    }

    private void OnEnable()
    {
        EventSystem.eventCollection[EventType.KRIS_EGG] += ShowKris;
    }

    private void OnDisable()
    {
        EventSystem.eventCollection[EventType.KRIS_EGG] -= ShowKris;
    }

    private void Update()
    {
        if (!show)
        {
            return;
        }

        if (shouldWait && waitTime >= 0)
        {
            waitTime -= Time.deltaTime;
        }

        if (shouldWait && waitTime <= 0)
        {
            shouldWait = false;
            krisObject.SetActive(true);
        }

        if (!shouldWait && shouldShow && showTime >= 0)
        {
            showTime -= Time.deltaTime;
        }

        if (shouldShow && showTime <= 0)
        {
            shouldShow = false;
            krisObject.SetActive(false);
        }
    }

    private void ShowKris()
    {
        shouldWait = true;
        shouldShow = true;
        waitTime = Random.Range(0, 100) / 100f;
        showTime = Random.Range(50, 150) / 100f;
    }

}
