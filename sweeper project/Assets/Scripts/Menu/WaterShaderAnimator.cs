using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterShaderAnimator : MonoBehaviour
{
    private float offset;
    private Material myMat;

    private void Start()
    {
        myMat = gameObject.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        offset += (Time.deltaTime / 100f);

        myMat.SetTextureOffset("_BaseColorMap", new Vector2(offset / 3f, offset));
    }
}