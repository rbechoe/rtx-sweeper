using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCraft : MonoBehaviour, ITriggerable
{
    public Transform nodeParent;
    public List<GameObject> nodes = new List<GameObject>();

    private bool fly;

    public float flightSpeed;
    public int target;
    private int maxTarget;

    public void Activate()
    {
        print("starting flight animation");
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

    // Update is called once per frame
    void Update()
    {
        if (fly)
        {
            // look with delay to target
            // fly forward on the -Z axis
        }
    }
}
