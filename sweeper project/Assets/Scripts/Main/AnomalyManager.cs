using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyManager : MonoBehaviour
{
    public Button lvl1, lvl2, lvl3;
    public GameObject puzzle1, puzzle2, puzzle3;
    public WallGenerator wallGenerator;
    private DataSerializer serializer;

    private void Awake()
    {
        serializer = GetComponent<DataSerializer>();
        AccountData userData = serializer.GetUserData();

        if (userData.unlockedAnomaly1)
        {
            lvl1.interactable = true;
        }

        if (userData.unlockedAnomaly2)
        {
            lvl2.interactable = true;
        }

        if (userData.unlockedAnomaly3)
        {
            lvl3.interactable = true;
        }
    }

    public void StartPuzzle1()
    {
        puzzle1.SetActive(true);
        puzzle2.SetActive(false);
        puzzle3.SetActive(false);
        wallGenerator.OpenWall();
    }

    public void StartPuzzle2()
    {
        puzzle1.SetActive(false);
        puzzle2.SetActive(true);
        puzzle3.SetActive(false);
        wallGenerator.OpenWall();
    }

    public void StartPuzzle3()
    {
        puzzle1.SetActive(false);
        puzzle2.SetActive(false);
        puzzle3.SetActive(true);
        wallGenerator.OpenWall();
    }

    public void CloseWall()
    {
        wallGenerator.CloseWall();
    }
}
