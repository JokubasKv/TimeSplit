using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
