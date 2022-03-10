using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
    public GameObject locked, unlocked;

    public bool delayed;

    private void Start()
    {
        if (delayed)
        {
            DelayedMethods.FireMethod(DelayedLocker, 1);
        }
        else
        {
            locked.SetActive(true);
            unlocked.SetActive(false);
        }
    }

    private void DelayedLocker()
    {
        locked.SetActive(true);
        unlocked.SetActive(false);
    }

    public void UnlockAreas()
    {
        locked.SetActive(false);
        unlocked.SetActive(true);
    }
}
