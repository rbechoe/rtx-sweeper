using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LookAt : Base
{
    private GameObject target;
    public TMP_Text bombCountTMP;
    private LayerMask selectionLayers;
    public Color defaultMid;
    public Color defaultNone;
    public Color defaultSide;

    protected override void Start()
    {
        target = GameObject.FindGameObjectWithTag("LookTarget");
        bombCountTMP = GetComponentInChildren<TMP_Text>();
        defaultMid = new Color(0.25f, 1, 0.25f, 0.5f);
        defaultNone = new Color(0.1f, 0.1f, 0.1f, 0.01f);
        defaultSide = new Color(1, 1, 1, 0.1f);
        selectionLayers = LayerMask.GetMask("Selector", "Transparent");
    }

    protected override void Update()
    {
        transform.LookAt(target.transform.position);

        Collider[] selectorType = Physics.OverlapBox(transform.position, Vector3.one * 0.1f, Quaternion.identity, selectionLayers);

        if (selectorType.Length > 0)
        {
            foreach (Collider col in selectorType)
            {
                if (col.CompareTag("Transparent"))
                {
                    bombCountTMP.color = defaultSide;
                    break;
                }

                if (col.CompareTag("Opaque"))
                {
                    bombCountTMP.color = defaultMid;
                    break;
                }
            }
        }
        else
        {
            bombCountTMP.color = defaultNone;
        }
    }
}