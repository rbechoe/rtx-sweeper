using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInitiate : MonoBehaviour
{
    public GameObject target;

    public float triggerDistance = 20;

    private bool triggerEntered;

    Collider other;

    private void Update()
    {
        if (triggerEntered && Vector3.Distance(transform.position, other.transform.position) < triggerDistance)
        {
            target.GetComponent<ITriggerable>()?.Activate();
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("LookTrigger"))
        {
            other = _other;
            triggerEntered = true;
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.CompareTag("LookTrigger"))
        {
            other = null;
            triggerEntered = false;
        }
    }
}
