using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManipulator : MonoBehaviour
{
    [Header("Debug settings")]
    public int bombCount;
    public bool forceUpdate;

    [Header("Assignables")]
    public Renderer shapeMat;
    public Renderer pulseMat1;
    public Renderer pulseMat2;
    public Texture2D[] textures;
    public GameObject gridTile;
    public GameObject bombEffect;

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

        float sub = bombCount / 5f;
        Color color = (bombCount < 5) ? new Color(0f + sub, 1, 0) : new Color(1, 1f - sub / 2f, 0);

        shapeMat.material.SetTexture("_MainMask", textures[bombCount - 1]);
        shapeMat.material.SetColor("_RampColorTint", color);
        pulseMat1.material.SetTexture("_MainMask", textures[bombCount - 1]);
        pulseMat1.material.SetColor("_RampColorTint", color);
        pulseMat2.material.SetTexture("_MainMask", textures[bombCount - 1]);
        pulseMat2.material.SetColor("_RampColorTint", color);
    }
}
