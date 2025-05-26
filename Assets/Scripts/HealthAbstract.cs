using UnityEngine;

public abstract class HealthAbstract : MonoBehaviour
{
    public float health;
    public float maxHealth;

    protected virtual void Start()
    {
        health = maxHealth;
        OnHealthChanged();

        InitStart();
    }

    public virtual void TakeDamage(float damage, DamageMethod damageMethod = DamageMethod.Unknown)
    {
        health -= damage;
        OnHealthChanged();
        OnDamageTaken(damage, damageMethod);

        if (health <= 0)
        {
            OnDeath();
        }
    }

    public virtual void SetHealth(float healthToSet)
    {
        health = healthToSet;
        OnHealthChanged();

        if (health <= 0)
        {
            OnDeath();
        }
    }

    protected abstract void OnHealthChanged();
    protected abstract void OnDamageTaken(float damage, DamageMethod damageMethod);
    protected abstract void OnDeath();
    protected virtual void InitStart()
    {

    }
}
