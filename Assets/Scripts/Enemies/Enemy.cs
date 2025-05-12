using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected StateMachine _stateMachine;
    private NavMeshAgent _agent;
    private GameObject _player;
    [SerializeField]
    private Vector3 _lastKnownPlayerPosition;

    public NavMeshAgent Agent { get => _agent; }
    public GameObject Player { get => _player; }
    public Vector3 LastKnownPlayerPosition { get => _lastKnownPlayerPosition; set => _lastKnownPlayerPosition = value; }

    public EnemyPath path;

    [Header("Sight")]
    public float sightDistance = 20f;
    public float fieldOfView = 80f;
    public float eyeHeight = 0f;

    [Header("Weapon")]
    public Transform gunBarrel;
    public float fireRate = 1f;

    [Header("Health")]
    public float maxHealth = 10f;
    public float health;

    [SerializeField]
    private string _currentState;

    private Renderer _renderer;
    private Color _originalColor;
    [SerializeField]
    private Color _damageFlashColor = Color.red;
    [SerializeField]
    private float _flashDuration = 0.2f;

    void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _renderer = GetComponentInChildren<Renderer>();

        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
        }
        health = maxHealth;
    }

    void Update()
    {
        _currentState = _stateMachine.activeState.ToString();

    }

    public bool CanSeePlayer()
    {
        if (_player == null) return false;

        if (Vector3.Distance(transform.position, _player.transform.position) < sightDistance)
        {
            Vector3 targetDirecion = _player.transform.position - transform.position - (Vector3.up * eyeHeight);
            float angleToPlayer = Vector3.Angle(targetDirecion, transform.forward);

            if (angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
            {
                Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirecion);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, sightDistance))
                {
                    if (hitInfo.transform.gameObject == _player)
                    {
                        Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (_renderer != null)
        {
            StartCoroutine(FlashDamage());
        }

        if (health <= 0)
        {
            DestroyEnemy();
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
}