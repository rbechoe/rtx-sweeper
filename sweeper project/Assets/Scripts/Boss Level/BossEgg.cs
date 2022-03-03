using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEgg : MonoBehaviour
{
    public GameObject mainCam, otherCam;
    bool switchCam;
    float cd = 0;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.W) && cd <= 0)
        {
            switchCam = !switchCam;
            cd = 0.5f;

            if (switchCam)
            {
                mainCam.SetActive(false);
                otherCam.SetActive(true);
            }
            else
            {
                mainCam.SetActive(true);
                otherCam.SetActive(false);
            }
        }

        if (cd > 0) cd -= Time.deltaTime;
    }
}
