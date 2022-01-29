using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInitiate : MonoBehaviour
{
    public GameObject target;

    Collider other;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target.GetComponent<ITriggerable>()?.Activate();
            gameObject.SetActive(false);
        }
    }
}
