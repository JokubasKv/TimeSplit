using System.Collections;
using UnityEngine;


public class EnemyHealth : HealthAbstract
{
    private Renderer[] _renderers;
    private Color[] _originalColors;
    [SerializeField]
    private Color _damageFlashColor = Color.red;
    [SerializeField]
    private float _flashDuration = 0.2f;

    [SerializeField]
    private EnemyType _enemyType = EnemyType.Unknown;

    private DamageMethod _lastDamageMethod = DamageMethod.Unknown;

    protected override void InitStart()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        if (_renderers != null && _renderers.Length > 0)
        {
            _originalColors = new Color[_renderers.Length];
            for (int i = 0; i < _renderers.Length; i++)
            {
                _originalColors[i] = _renderers[i].material.color;
            }
        }
    }

    private void OnEnable()
    {
        if (_renderers != null && _originalColors != null)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material.color = _originalColors[i];
            }
        }
    }

    private IEnumerator FlashDamage()
    {
        if (_renderers != null)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material.color = _damageFlashColor;
            }
            yield return new WaitForSeconds(_flashDuration);
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material.color = _originalColors[i];
            }
        }
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
