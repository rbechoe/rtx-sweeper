using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInitiate : MonoBehaviour
{
    public GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LookTrigger"))
        {
            target.GetComponent<ITriggerable>()?.Activate();
            gameObject.SetActive(false);
        }
    }
}
