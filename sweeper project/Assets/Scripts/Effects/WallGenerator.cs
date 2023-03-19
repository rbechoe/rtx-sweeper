using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public GameObject wallPiece;
    public int height = 20;
    public int width = 20;

    public List<GameObject> walls = new List<GameObject>();
    public List<GameObject> firsthalf = new List<GameObject>();
    public List<GameObject> secondhalf = new List<GameObject>();

    public bool inverseSecondDoor;
    public float valueToRotateTo = 0;
    public float currentRotation = 0;

    public GameObject vortex;

    void Awake()
    {
        for (int x = 0; x < width; x++)
        {
            GameObject pillar = Instantiate(wallPiece, new Vector3(x, 0, 0), Quaternion.identity);
            pillar.name = "Pillar " + x;

            for (int y = 0; y < height; y++)
            {
                float xPos = (y % 2 == 1) ? x : x + 0.5f;
                GameObject piece = Instantiate(wallPiece, new Vector3(xPos, y, 0), Quaternion.identity);
                piece.transform.parent = pillar.transform;
            }

            walls.Add(pillar);
        }

        for (int i = 0; i < walls.Count / 2; i++)
        {
            try
            {
                if (i != 0)
                {
                    walls[i].transform.parent = walls[i - 1].transform;
                    firsthalf.Add(walls[i]);
                }

                walls[walls.Count / 2 + i].transform.parent = walls[walls.Count / 2 + i + 1].transform;
                secondhalf.Add(walls[walls.Count / 2 + i]);
            }
            catch { }
        }
    }

    [ContextMenu("Open")]
    public void OpenWall()
    {
        valueToRotateTo = -20;
    }

    [ContextMenu("Close")]
    public void CloseWall()
    {
        valueToRotateTo = 0;
    }

    private void FixedUpdate()
    {
        if (currentRotation != valueToRotateTo)
        {
            float speed = Mathf.Clamp(Mathf.Abs((currentRotation - valueToRotateTo)) / 100f, 0.005f, 0.25f);
            if (currentRotation < valueToRotateTo)
            {
                currentRotation += speed;
            }

            if (currentRotation > valueToRotateTo)
            {
                currentRotation -= speed;
            }
        }

        for (int i = 0; i < firsthalf.Count; i++)
        {
            firsthalf[i].transform.localEulerAngles = new Vector3(0, currentRotation, 0);
            if (inverseSecondDoor)
            {
                secondhalf[secondhalf.Count - i - 1].transform.localEulerAngles = new Vector3(0, currentRotation, 0);
            }
            else
            {
                secondhalf[secondhalf.Count - i - 1].transform.localEulerAngles = new Vector3(0, -currentRotation, 0);
            }
        }
    }
}
