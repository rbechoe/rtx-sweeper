using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTile : MonoBehaviour
{
    public Color defaultCol = Color.grey;
    public Color selectCol = Color.green;
    
    private Material myMat;
    private AudioSource audioSource;

    private bool clickable;
    private bool previewClicked;
    private bool canReveal;

    void Start()
    {
        myMat = gameObject.GetComponent<Renderer>().material;
        myMat.EnableKeyword("_EMISSION");
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
        audioSource = gameObject.GetComponent<AudioSource>();
        clickable = true;
    }

    private void OnMouseOver()
    {
        if (clickable)
        {
            myMat.color = selectCol;
            myMat.SetColor("_EmissiveColor", selectCol * 10);
        }

        // release left button - reveal tile
        if (Input.GetMouseButtonUp(0))
        {
            if (clickable)
            {
                defaultCol = Color.red;
                myMat.color = defaultCol;
                myMat.SetColor("_EmissiveColor", defaultCol);
                audioSource.Play();
                clickable = false;
            }
        }
    }

    private void OnMouseExit()
    {
        // set tile back to base color
        if (clickable)
        {
            myMat.color = defaultCol;
            myMat.SetColor("_EmissiveColor", defaultCol);
        }
    }
}
