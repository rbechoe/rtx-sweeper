using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VFXManipulator : MonoBehaviour
{
    [Header("Debug settings")]
    public int bombCount;
    public bool forceUpdate;

    [Header("Assignables")]
    public Renderer shapeMat;
    public Texture2D texture;
    public GameObject gridTile;
    //public GameObject bombEffect;
    public TextMeshPro text;

    void Update()
    {
        if (forceUpdate)
        {
            forceUpdate = false;
            UpdateEffect(bombCount);
        }
    }

    public void UpdateEffect(int bombCount)
    {
        if (bombCount <= 0) bombCount = 9; // 9th entry = 0

        if (text != null)
        {
            if (bombCount == 9)
                text.text = "";
            else
                text.text = "" + bombCount;
        }

        float sub = bombCount / 5f;
        Color color;// = (bombCount < 5) ? new Color(0f + sub, 1, 0) : new Color(1, 1f - sub / 2f, 0);

        // assign color per bomb amount
        switch (bombCount)
        {
            default:
                color = Color.white;
                break;
            case 1:
                color = new Color(0, 1, 0);
                break;
            case 2:
                color = new Color(.5f, 1, 0);
                break;
            case 3:
                color = new Color(1, 1, 0);
                break;
            case 4:
                color = new Color(1, .5f, 0);
                break;
            case 5:
                color = new Color(1, 0, 0);
                break;
            case 6:
                color = new Color(1, 0, .5f);
                break;
            case 7:
                color = new Color(.5f, 0, 1);
                break;
            case 8:
                color = new Color(0, 0, 1);
                break;
        }

        if (shapeMat == null) return;
        shapeMat.material.SetTexture("_MainMask", texture);
        shapeMat.material.SetColor("_RampColorTint", color);

        text.color = color;
    }
}
