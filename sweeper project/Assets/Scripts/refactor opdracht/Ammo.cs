using UnityEngine;

public abstract class Ammo : MonoBehaviour
{
    public virtual void DestroyAmmo()
    {
        Destroy(gameObject);
    }
}