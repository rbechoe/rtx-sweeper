using UnityEngine;
using UnityEngine.UI;

public class AnomalyChecker : MonoBehaviour
{
    public int anomaly = 1;
    public DataSerializer serializer;
    private AccountData myData;

    public Text text;

    private void Start()
    {
        myData = serializer.GetUserData();

        if (myData.bossVictories > 0)
        {
            switch (anomaly)
            {
                case 1:
                    if (myData.unlockedAnomaly1 == true)
                    {
                        text.text = "1/1";
                    }
                    else
                    {
                        text.text = "0/1";
                    }
                    return;
                case 2:
                    if (myData.unlockedAnomaly2 == true)
                    {
                        text.text = "1/1";
                    }
                    else
                    {
                        text.text = "0/1";
                    }
                    return;
                case 3:
                    if (myData.unlockedAnomaly3 == true)
                    {
                        text.text = "1/1";
                    }
                    else
                    {
                        text.text = "0/1";
                    }
                    return;
            }
        }

        text.gameObject.SetActive(false);
    }
}
