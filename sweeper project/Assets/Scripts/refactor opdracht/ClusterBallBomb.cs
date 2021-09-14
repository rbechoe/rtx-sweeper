using System.Collections.Generic;
using UnityEngine;

public class ClusterBallBomb : Ammo, IBullet
{
    public GameObject[] missiles;

    private AudioHandler AH;
    private GameStatistics GS;

    // set properties when created
    private void Start()
    {
        damage = 1;
        timeAlive = 5f;
        bulletSpeed = 1000;
        explosionRadius = GlobalData.WeaponRange;

        AH = GameObject.FindGameObjectWithTag("AudioHandler").GetComponent<AudioHandler>();
        GS = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameStatistics>();
    }

    private void Update()
    {
        timeAlive -= Time.deltaTime;
        if (timeAlive <= 0)
        {
            DestroyAmmo();
        }
    }

    // when bomb hits enemy explode and shoot homing missiles
    private void OnCollisionEnter(Collision other)
    {
        // Only explode when other object doesnt contain shield part
        if (other.gameObject.GetComponent<ShieldPart>() == null)
        {
            Explosion(other.gameObject);
        }

        // Get all potential targets
        List<GameObject> targets = GetNearbyEnemies();

        // Assign a target to a mini bomb
        int target = 0;
        foreach (GameObject missile in missiles)
        {
            // skip empty entries
            if (missile == null)
            {
                continue;
            }

            // it is possible for 1+ targets to get multiple hits
            if (target >= targets.Count)
            {
                target = 0; 
            }

            // activate a missle
            ActivateMissile(missile, targets[target]);
            
            target++;
        }

        DestroyAmmo();
    }

    // Get all potential targets in vicinity
    private List<GameObject> GetNearbyEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        List<GameObject> targets = new List<GameObject>();

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("Enemy"))
            {
                targets.Add(hitColliders[i].gameObject);
            }
        }

        return targets;
    }

    // bomb collides with something it can explode on
    private void Explosion(GameObject hitObject)
    {
        // boom effect
        GameObject splash = Instantiate(hitEffect, transform.position, Quaternion.identity) as GameObject;
        splash.name = "splash effect";
        AH.BorrowChild(transform.position, hitSfx);
        GS.Explosion(Vector3.Distance(transform.position, Vector3.zero));

        // give damage to object it impacted in if it can
        hitObject.gameObject.GetComponent<IDamagable>()?.TakeDamage(damage);
    }

    // activate a missile and assign target for homing
    private void ActivateMissile(GameObject missile, GameObject target)
    {
        // enable missile collision (missile is always visible, but gets activated)
        missile.GetComponent<SphereCollider>().enabled = true;
        missile.transform.parent = null;

        // shoot missile away before it homes into enemy
        Rigidbody rb = missile.GetComponent<Rigidbody>();
        if (rb == null)
        {
            missile.AddComponent<Rigidbody>();
            rb = missile.GetComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.AddForce(missile.transform.forward * bulletSpeed);

        // setup missile
        ClusterMissile CM = missile.GetComponent<ClusterMissile>();
        CM.enabled = true;
        CM.damage = damage;
        CM.target = target;
    }

    // set the effects
    public void SetEffects(GameObject _hitEffect, AudioClip _hitSfx)
    {
        hitEffect = _hitEffect;
        hitSfx = _hitSfx;
    }

    // update shots fired and destroy the bomb
    public void DestroyBullet()
    {
        GS.IncrementShots(missiles.Length);
        DestroyAmmo();
    }
}