using UnityEngine;

public interface IBullet
{
    void DestroyBullet();
    void SetEffects(GameObject _hitEffect, AudioClip _hitSfx);
}