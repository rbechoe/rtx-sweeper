using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInitiate : MonoBehaviour
{
    public GameObject target;

    public float triggerDistance = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LookTrigger") && Vector3.Distance(transform.position, other.transform.position) < triggerDistance)
        {
            target.GetComponent<ITriggerable>()?.Activate();
            gameObject.SetActive(false);
        }
    }
}
