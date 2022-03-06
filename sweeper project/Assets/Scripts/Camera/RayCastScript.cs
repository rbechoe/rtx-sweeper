using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastScript : MonoBehaviour
{
    public GameObject target;
    public LayerMask selector;

    private void Update()
    {
        RaycastHit hit;
        Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, int.MaxValue, selector))
        {
            target.transform.position = hit.point;
        }
    }
}
