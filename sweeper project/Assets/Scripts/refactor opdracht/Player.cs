using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private IDamageable healthComponent;
    public ICanAttack currentWeapon;

    private void Start()
    {
        healthComponent = GetComponent<IDamageable>();
        ObtainWeapon(new Bow());
    }

    public void ObtainWeapon(ICanAttack _weapon)
    {
        currentWeapon = _weapon;
    }
}

public interface IDamageable
{
    float Health { get; }
    void TakeDamage(float damage);
}

public class HealthComponent : MonoBehaviour, IDamageable
{    
    public float Health { get; private set; }
    [SerializeField] private float maxHealth;
    [SerializeField] private PlayerUI playerUI;

    public void TakeDamage(float _damage)
    {
        Health -= _damage;
        playerUI.UpdateHealthUI(Health, maxHealth);
        if (Health <= 0) { Die(); }
    }

    private void Die()
    {

    }
}

public interface ICanAttack
{
    void Attack();
}

public abstract class Weapon : MonoBehaviour, ICanAttack
{
    public abstract void Attack();
}

public class Sword : Weapon
{
    public override void Attack()
    {
        // swoosh
    }
}

public class Bow : Weapon
{
    public override void Attack()
    {
        // pew
    }
}