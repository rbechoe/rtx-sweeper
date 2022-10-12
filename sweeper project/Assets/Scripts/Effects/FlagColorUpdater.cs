using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagColorUpdater : MonoBehaviour
{
    Material myMat;
    public Texture2D[] flags;
    /* * flag order
     * normal
     * alien
     * cracked
     * fancy cross
     * plus
     * round
     * */

    void Start()
    {
        myMat = GetComponent<ParticleSystemRenderer>().material;
        myMat.SetColor("_RampColorTint", Settings.Instance.GetFlagColor());
        myMat.SetTexture("_MainMask", flags[Settings.Instance.GetFlagIndex()]);
    }
}
