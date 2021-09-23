using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : Base
{
    private GameObject target;

    protected override void Start()
    {
        target = GameObject.FindGameObjectWithTag("LookTarget");
    }

    protected override void Update()
    {
        transform.LookAt(target.transform.position);
    }
}