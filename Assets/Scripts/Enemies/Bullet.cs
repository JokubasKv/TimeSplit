using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public float damage = 10f;
    [SerializeField] public DamageMethod damageMethod = DamageMethod.Bullet;

    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;
        var healthAbstract = hitTransform.GetComponent<HealthAbstract>();
        if (healthAbstract != null)
        {
            healthAbstract.TakeDamage(damage, damageMethod);
        }

        gameObject.SetActive(false);
    }
}
