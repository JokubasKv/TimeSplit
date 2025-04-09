using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private StateMachine _stateMachine;
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

    [SerializeField]
    private string _currentState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _stateMachine = GetComponent<StateMachine>();
        _agent = GetComponent<NavMeshAgent>();
        _stateMachine.Initialize();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        CanSeePlayer();
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
}
