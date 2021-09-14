using System.Collections.Generic;
using UnityEngine;

public class ClusterBallBomb : Ammo, IBullet
{
    AudioHandler AH;
    GameStatistics GS;

    public GameObject[] miniBalls;

    public int bulletSpeed { get; set; }
    public int damage { get; set; }
    public float timeAlive { get; set; }
    public float explosionRadius { get; set; }

    public GameObject hitEffect { get; set; }

    void Start()
    {
        AH = GameObject.FindGameObjectWithTag("AudioHandler").GetComponent<AudioHandler>();
        GS = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameStatistics>();

        timeAlive = 5f;
        damage = 1;
        bulletSpeed = 1000;
        explosionRadius = GlobalData.WeaponRange;
    }

    void Update()
    {
        timeAlive -= Time.deltaTime;
        if (timeAlive <= 0)
        {
            DestroyBullet();
        }
    }

    public override void DestroyAmmo()
    {
        GS.shotsFired += 4;
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        // Only explode when other object doesnt contain shield part
        if (other.gameObject.GetComponent<ShieldPart>() == null)
        {
            GameObject bld = Instantiate(hit, transform.position, Quaternion.identity) as GameObject;
            bld.name = "splat";
            other.gameObject.GetComponent<DamageReceiver>().ReceiveDamage(damage);
            AH.BorrowChild(transform.position, "rocket");
            GS.Explosion(Vector3.Distance(transform.position, Vector3.zero));
        }

        // Get all potential targets
        List<GameObject> targets = GetNearbyEnemies();

        // Assign targets to mini bombs
        int count = 0;
        int targ = 0;
        foreach (GameObject ball in miniBalls)
        {
            if (ball == null)
                continue;
            if (targ >= targets.Count)
                targ = 0;

            ball.GetComponent<SphereCollider>().enabled = true;
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb == null)
            {
                ball.AddComponent<Rigidbody>();
                rb = ball.GetComponent<Rigidbody>();
            }
            rb.useGravity = false;
            rb.AddForce(ball.transform.forward * bulletSpeed);
            ClusterMissile CM = ball.GetComponent<ClusterMissile>();
            CM.enabled = true;
            CM.damage = damage;
            CM.target = targets[targ];
            ball.transform.parent = null;
            count++;
            targ++;
        }
        DestroyAmmo();
    }

    // Get all potential targets
    List<GameObject> GetNearbyEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        int i = 0;
        List<GameObject> targets = new List<GameObject>();
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Enemy")
                targets.Add(hitColliders[i].gameObject);
            i++;
        }

        return targets;
    }
}