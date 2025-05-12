using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;

    void Start()
    {
        health = maxHealth;
        UIManager.instance.SetHealthBar(health / maxHealth);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        UIManager.instance.SetHealthBar(health / maxHealth);

        if (health <= 0)
        {
            LevelManager.instance.ResetGame();
        }
    }
}
