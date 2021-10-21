using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCraft : MonoBehaviour, ITriggerable
{
    public Transform nodeParent;
    public List<GameObject> nodes = new List<GameObject>();

    public bool fly;

    public float flightSpeed = 100;
    public float rotateSpeed = 5;
    public int target;
    public int maxTarget;

    public void Activate()
    {
        fly = true;
    }

    void Start()
    {
        foreach (Transform child in nodeParent)
        {
            nodes.Add(child.gameObject);
        }
        maxTarget = nodes.Count;
    }

    void Update()
    {
        if (fly)
        {
            Vector3 targetDir = nodes[target].transform.position - transform.position;
            float step = Time.deltaTime * rotateSpeed;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            transform.position += transform.forward * flightSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, nodes[target].transform.position) < 5)
            {
                if (target < nodes.Count - 1)
                {
                    target++;
                }
            }
            else
            {
                //gameObject.SetActive(false);
            }
        }
    }
}
