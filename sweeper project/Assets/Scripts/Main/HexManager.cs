using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexManager : MonoBehaviour
{
    public GameObject hexPrefab;
    public List<GameObject> hexList = new List<GameObject>();

    public int height = 20;
    public int width = 30;

    public HexRow[] hexRows;
    public float[] hexFloats;
    private int zeroDepthIndex = 0;
    private int firstDepthIndex = 1;
    private int secondDepthIndex = 2;
    private int thirdDepthIndex = 3;
    private int fourthDepthIndex = 4;

    private float waveCd = 0.1f;

    private void Start()
    {
        hexFloats = new float[width];
        hexRows = new HexRow[width];

        for (int y = 0; y < height; y++)
        {
            Vector3 position = new Vector3(0, y, 0);
            for (int x = 0; x < width; x++)
            {
                if (y == 0)
                {
                    hexRows[x].rowList = new List<GameObject>();
                    hexRows[x].index = x;
                }

                position.x = (y % 2 == 0) ? x : x + 0.5f;
                position.x *= 1.1f;
                GameObject hex = Instantiate(hexPrefab, transform.position, Quaternion.identity);
                hex.transform.parent = transform;
                hex.transform.localEulerAngles = new Vector3(270, 0, 0);
                hex.transform.localPosition = position;
                hexList.Add(hex);

                hexRows[x].rowList.Add(hex);
            }
        }
    }

    private void Update()
    {
        waveCd -= Time.deltaTime;

        if (waveCd <= 0)
        {
            waveCd = 0.1f;

            zeroDepthIndex++;
            firstDepthIndex++;
            secondDepthIndex++;
            thirdDepthIndex++;
            fourthDepthIndex++;

            if (zeroDepthIndex >= hexFloats.Length)
            {
                zeroDepthIndex = 0;
            }
            if (firstDepthIndex >= hexFloats.Length)
            {
                firstDepthIndex = 0;
            }
            if (secondDepthIndex >= hexFloats.Length)
            {
                secondDepthIndex = 0;
            }
            if (thirdDepthIndex >= hexFloats.Length)
            {
                thirdDepthIndex = 0;
            }
            if (fourthDepthIndex >= hexFloats.Length)
            {
                fourthDepthIndex = 0;
            }

            for (int i = 0; i < hexFloats.Length; i++)
            {
                hexFloats[i] = 0;
                if (i == firstDepthIndex || i == thirdDepthIndex)
                {
                    hexFloats[i] = -0.25f;
                }
                if (i == secondDepthIndex)
                {
                    hexFloats[i] = -0.5f;
                }
            }

            for (int i = 0; i < hexRows.Length; i++)
            {
                if (i != zeroDepthIndex && i != firstDepthIndex && i != secondDepthIndex && i != thirdDepthIndex && i != fourthDepthIndex)
                {
                    continue;
                }

                for (int j = 0; j < hexRows[i].rowList.Count; j++)
                {
                    Vector3 hexPos = hexRows[i].rowList[j].transform.localPosition;
                    hexPos.z = hexFloats[hexRows[i].index];
                    hexRows[i].rowList[j].transform.localPosition = hexPos;

                    HexTileAnomaly hexComponent = hexRows[i].rowList[j].GetComponent<HexTileAnomaly>();
                    if (hexComponent != null && hexComponent.state == TileStates.Bomb)
                    {
                        hexComponent.SpreadInfection();
                    }
                }
            }
        }
    }
}

[System.Serializable]
public struct HexRow
{
    public List<GameObject> rowList;
    public int index; // used for float list
}
