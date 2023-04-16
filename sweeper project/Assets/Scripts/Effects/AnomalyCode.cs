using UnityEngine;

public class AnomalyCode : MonoBehaviour
{
    public DataSerializer DS;
    public GameObject qr1, qr2, qr3;

    private void Start()
    {
        UpdateQR();
    }

    private void OnEnable()
    {
        EventSystem.eventCollection[EventType.WIN_GAME] += UpdateQR;
    }

    private void OnDisable()
    {
        EventSystem.eventCollection[EventType.WIN_GAME] -= UpdateQR;
    }

    private void UpdateQR()
    {
        AccountData data = DS.GetUserData();

        if (data.anomalyVictories1 > 0)
        {
            qr1.SetActive(true);
        }
        else
        {
            qr1.SetActive(false);
        }

        if (data.anomalyVictories2 > 0)
        {
            qr2.SetActive(true);
        }
        else
        {
            qr2.SetActive(false);
        }

        if (data.anomalyVictories3 > 0)
        {
            qr3.SetActive(true);
        }
        else
        {
            qr3.SetActive(false);
        }
    }
}
