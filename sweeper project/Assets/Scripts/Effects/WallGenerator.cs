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

    void Awake()
    {
        for (int x = 0; x < width; x++)
        {
            GameObject pillar = Instantiate(wallPiece, new Vector3(x, 0, 0), Quaternion.identity);
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
            if (i != 0)
            {
                walls[i].transform.parent = walls[i - 1].transform;
                firsthalf.Add(walls[i]);
            }
            walls[walls.Count / 2 + i].transform.parent = walls[walls.Count / 2 + i + 1].transform;
            secondhalf.Add(walls[walls.Count / 2 + i]);
        }
    }

    // TODO animate walls by rotating pillars!!
}
