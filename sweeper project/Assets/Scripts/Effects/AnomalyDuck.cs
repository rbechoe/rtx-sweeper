using UnityEngine;

public class AnomalyDuck : MonoBehaviour
{
    private SteamAPIManager steamAPI;
    public DataSerializer DS;
    private AccountData AD;

    private bool hovered = false;

    private void Start()
    {
        steamAPI = SteamAPIManager.Instance;
        AD = DS.GetUserData();

        if (AD.anomalyVictories1 == 0 || AD.anomalyVictories2 == 0 || AD.anomalyVictories3 == 0)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (hovered && Input.GetMouseButtonDown(0))
        {
            steamAPI.SetAchievement(UserAchievements.anomalySolved);
        }
    }

    private void OnMouseEnter()
    {
        hovered = true;
    }

    private void OnMouseExit()
    {
        hovered = false;
    }
}
