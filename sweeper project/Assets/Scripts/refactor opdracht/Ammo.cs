using UnityEngine;

public abstract class Ammo : MonoBehaviour
{
    public int bulletSpeed { get; set; }
    public int damage { get; set; }

    public float timeAlive { get; set; }
    public float explosionRadius { get; set; }

    public GameObject hitEffect { get; set; }
    public AudioClip hitSfx { get; set; }

    public virtual void DestroyAmmo()
    {
        Destroy(gameObject);
    }
}