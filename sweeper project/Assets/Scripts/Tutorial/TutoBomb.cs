using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoBomb : MonoBehaviour
{
    public GameObject target;
    public GameObject explosionEffect;
    private AudioSource audioSource;

    public GameObject[] triggerbles;

    public bool hasPurpose;

    private Vector3 targetPos;
    public float flightSpeed = 100;
    public float rotateSpeed = 5;

    private bool activated = false;
    private bool landed = false;

    void Start()
    {
        if (hasPurpose)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            targetPos = target.transform.position;

            foreach (GameObject triggerable in triggerbles)
            {
                triggerable.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (activated && !landed)
        {
            Vector3 targetDir = targetPos - transform.position;
            float step = Time.deltaTime * rotateSpeed;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            transform.position += transform.forward * flightSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPos) < 1)
            {
                StartCoroutine(LandBomb());
                landed = true;
            }
        }
    }

    public void Activate()
    {
        if (!activated && hasPurpose)
        {
            activated = true;
        }

        if (!activated && !hasPurpose)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    IEnumerator LandBomb()
    {
        audioSource.Play();
        transform.GetChild(0).gameObject.SetActive(false);
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        foreach (GameObject triggerable in triggerbles)
        {
            triggerable.SetActive(true);
        }
        yield return new WaitForEndOfFrame();
    }
}
