using UnityEngine;
using UnityEngine.SceneManagement;

public class AnomalyActivator : MonoBehaviour
{
    public int anomaly = 1;
    public DataSerializer serializer;
    private AccountData myData;

    private void Start()
    {
        myData = serializer.GetUserData();
        if (myData.bossVictories == 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0) && myData.bossVictories > 0)
        {
            switch(anomaly)
            {
                case 1:
                    myData.unlockedAnomaly1 = true;
                    break;
                case 2:
                    myData.unlockedAnomaly2 = true;
                    break;
                case 3:
                    myData.unlockedAnomaly3 = true;
                    break;
            }
            serializer.UpdateAccountData(myData);
            SceneManager.LoadScene("Anomaly");
        }
    }
}
