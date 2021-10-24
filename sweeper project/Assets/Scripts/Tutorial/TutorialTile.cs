using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialTile : MonoBehaviour
{
    public Color defaultCol = Color.grey;
    public Color selectCol = Color.green;
    public TMP_Text bombCountTMP;

    private MeshRenderer meshRenderer;
    private Material myMat;
    private AudioSource audioSource;
    
    private bool clickable;
    private bool previewClicked;
    private bool canReveal;

    void Start()
    {
        bombCountTMP = GetComponentInChildren<TMP_Text>();
        myMat = gameObject.GetComponent<Renderer>().material;
        myMat.EnableKeyword("_EMISSION");
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
        meshRenderer = bombCountTMP.gameObject.GetComponent<MeshRenderer>();
        audioSource = gameObject.GetComponent<AudioSource>();
        meshRenderer.enabled = false;
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
                defaultCol = Color.cyan;
                myMat.color = defaultCol;
                myMat.SetColor("_EmissiveColor", defaultCol);
                audioSource.Play();
                meshRenderer.enabled = true;
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
