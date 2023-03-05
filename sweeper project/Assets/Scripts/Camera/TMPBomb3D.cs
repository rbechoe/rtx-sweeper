using TMPro;
using UnityEngine;

public class TMPBomb3D : MonoBehaviour
{
    private GameObject target;
    private TMP_Text bombCountTMP;

    private bool centerLook;

    private Color defaultCol;
    private Color highlightCol = new Color(.5f, .5f, .5f, 1);

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("MainCamera");
        bombCountTMP = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (centerLook)
        {
            transform.localEulerAngles = Vector3.zero;
            transform.parent.LookAt(Vector3.zero);
        }
        else
        {
            transform.localEulerAngles = new Vector3 (0, 180, 0);
            transform.parent.LookAt(target.transform.position);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            centerLook = !centerLook;
        }
    }

    public void BombCount(int amount)
    {
        if (amount == 0)
        {
            bombCountTMP.text = "";
        }
        else
        {
            // assign color per bomb amount, inspired by original Minesweeper colors
            switch (amount)
            {
                case 1:
                    defaultCol = new Color(0, 1, 0);
                    break;
                case 2:
                    defaultCol = new Color(.5f, 1, 0);
                    break;
                case 3:
                    defaultCol = new Color(1, 1, 0);
                    break;
                case 4:
                    defaultCol = new Color(1, .5f, 0);
                    break;
                case 5:
                    defaultCol = new Color(1, 0, 0);
                    break;
                case 6:
                    defaultCol = new Color(1, 0, .5f);
                    break;
                case 7:
                    defaultCol = new Color(.5f, 0, 1);
                    break;
                default:
                case 8:
                    defaultCol = new Color(0, 0, 1);
                    break;
            }

            bombCountTMP.text = "" + amount;
            bombCountTMP.color = highlightCol;
        }
    }

    public void HighlightColor()
    {
        bombCountTMP.color = defaultCol;
    }

    public void DefaultColor()
    {
        bombCountTMP.color = highlightCol;
    }
}