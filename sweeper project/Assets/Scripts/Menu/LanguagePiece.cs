using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguagePiece : MonoBehaviour
{
    public GameObject selected;

    public void Select()
    {
        selected.SetActive(true);
    }

    public void Deselect()
    {
        selected.SetActive(false);
    }
}
