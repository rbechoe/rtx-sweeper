using UnityEngine;

public class GameStatistics : MonoBehaviour
{
    private int shotsFired;

    // increment total amount of shots fired
    public void IncrementShots(int _shotsFired)
    {
        shotsFired += _shotsFired;
    }

    // uses entity that handles camera shake -> CameraEffectsHandler
    public void Explosion(float distance)
    {
        //CEH.ExplosionImpact(distance);
    }
}