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

    private Vector3 ano1, ano2, ano3;

    private void Awake()
    {
        serializer = GetComponent<DataSerializer>();
        AccountData userData = serializer.GetUserData();

        ano1 = puzzle1.transform.position;
        ano2 = puzzle2.transform.position;
        ano3 = puzzle3.transform.position;

        puzzle1.transform.position = Vector3.up * 2000;
        puzzle2.transform.position = Vector3.up * 3000;
        puzzle3.transform.position = Vector3.up * 4000;

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
        puzzle1.transform.position = ano1;
        wallGenerator.OpenWall();
    }

    public void StartPuzzle2()
    {
        puzzle1.SetActive(false);
        puzzle2.SetActive(true);
        puzzle3.SetActive(false);
        puzzle2.transform.position = ano2;
        wallGenerator.OpenWall();
    }

    public void StartPuzzle3()
    {
        puzzle1.SetActive(false);
        puzzle2.SetActive(false);
        puzzle3.SetActive(true);
        puzzle3.transform.position = ano3;
        wallGenerator.OpenWall();
    }

    public void CloseWall()
    {
        wallGenerator.CloseWall();
    }
}
