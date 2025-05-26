using System.Collections;
using UnityEngine;


public class EnemyHealth : HealthAbstract
{
    private Renderer _renderer;
    private Color _originalColor;
    [SerializeField]
    private Color _damageFlashColor = Color.red;
    [SerializeField]
    private float _flashDuration = 0.2f;

    [SerializeField]
    private EnemyType _enemyType = EnemyType.Unknown;

    private DamageMethod _lastDamageMethod = DamageMethod.Unknown;

    protected override void InitStart()
    {
        _renderer = GetComponentInChildren<Renderer>();

        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
        }
    }

    private IEnumerator FlashDamage()
    {
        _renderer.material.color = _damageFlashColor;
        yield return new WaitForSeconds(_flashDuration);
        _renderer.material.color = _originalColor;
    }

    private void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }

    protected override void OnHealthChanged()
    {
    }

    protected override void OnDamageTaken(float damage, DamageMethod damageMethod)
    {
        StartCoroutine(FlashDamage());
        _lastDamageMethod = damageMethod;
    }

    protected override void OnDeath()
    {
        AwardPoints();
        DestroyEnemy();
    }

    private void AwardPoints()
    {
        PointsController.instance.AddEnemyKillPoints(_enemyType, _lastDamageMethod);
    }
}
