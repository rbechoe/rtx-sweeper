using TMPro;
using UnityEngine;

public class TMPBomb3D : MonoBehaviour
{
    private GameObject target;
    public TMP_Text bombCountTMP;

    private bool centerLook;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("MainCamera");
        bombCountTMP = GetComponentInChildren<TMP_Text>();
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
            Color color = Color.white;
            switch (amount)
            {
                case 1:
                    color = new Color(0, 1, 0);
                    break;
                case 2:
                    color = new Color(.5f, 1, 0);
                    break;
                case 3:
                    color = new Color(1, 1, 0);
                    break;
                case 4:
                    color = new Color(1, .5f, 0);
                    break;
                case 5:
                    color = new Color(1, 0, 0);
                    break;
                case 6:
                    color = new Color(1, 0, .5f);
                    break;
                case 7:
                    color = new Color(.5f, 0, 1);
                    break;
                default:
                case 8:
                    color = new Color(0, 0, 1);
                    break;
            }

            bombCountTMP.text = "" + amount;
            bombCountTMP.color = color;
        }
    }
}