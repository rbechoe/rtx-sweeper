using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewColor : MonoBehaviour
{
    RawImage myImg;
    public RawImage newImg;
    // TODO add array of flag textures to go through

    void Start()
    {
        myImg = GetComponent<RawImage>();
        myImg.color = Settings.Instance.GetFlagColor();
    }

    public void UpdateColor()
    {
        myImg.color = newImg.color;
    }
}
