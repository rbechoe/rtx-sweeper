using UnityEngine;

public interface IBullet
{
    public int bulletSpeed { get; set; }
    public int damage { get; set; }
    public float timeAlive { get; set; }
    public float explosionRadius { get; set; }

    public GameObject hitEffect { get; set; }

    public void DestroyBullet();
}