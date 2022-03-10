using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
    public GameObject locked, unlocked;

    private void Start()
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
