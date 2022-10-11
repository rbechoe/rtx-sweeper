using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagColorUpdater : MonoBehaviour
{
    Material myMat;

    void Start()
    {
        myMat = GetComponent<ParticleSystemRenderer>().material;
        myMat.SetColor("_RampColorTint", Settings.Instance.GetFlagColor());
    }
}
