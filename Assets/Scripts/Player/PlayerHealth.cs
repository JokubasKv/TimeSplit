public class PlayerHealth : HealthAbstract
{
    protected override void OnHealthChanged()
    {
        UIManager.instance.SetHealthBar(health / maxHealth);
    }

    protected override void OnDamageTaken(float damage, DamageMethod damageMethod)
    {
        UIManager.instance.TriggerHurt();
    }

    protected override void OnDeath()
    {
        UIManager.instance.GameOver();
    }
}
