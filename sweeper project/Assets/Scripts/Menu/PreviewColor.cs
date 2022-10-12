using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewColor : MonoBehaviour
{
    public RawImage newImg;
    public RawImage myImg;
    public Texture2D[] flags;
    /* * flag order
     * normal
     * alien
     * cracked
     * fancy cross
     * plus
     * round
     * */

    private int index = 0; // store this value and load it as well

    void Start()
    {
        index = Settings.Instance.GetFlagIndex();
        myImg.color = Settings.Instance.GetFlagColor();
        myImg.texture = flags[index];
    }

    public void UpdateColor()
    {
        myImg.color = newImg.color;
    }

    public void NextFlag()
    {
        index++;
        if (index >= flags.Length - 1) index = 0;
        myImg.texture = flags[index];
        Settings.Instance.SetFlagIndex(index);
    }

    public void PreviousFlag()
    {
        index--;
        if (index <= 0) index = flags.Length - 1;
        myImg.texture = flags[index];
        Settings.Instance.SetFlagIndex(index);
    }
}
