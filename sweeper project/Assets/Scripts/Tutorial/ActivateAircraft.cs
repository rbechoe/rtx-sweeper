using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAircraft : MonoBehaviour, ITriggerable
{
    public void Activate()
    {
        print("aircraft triggered");
    }
}
