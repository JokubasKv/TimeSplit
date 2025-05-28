using UnityEngine;

public class PatrollingEnemy : Enemy
{
    void Start()
    {
        _stateMachine.Initialize(new PatrolState());
    }

    public override void LookAt(Vector3 position)
    {
        Vector3 targetPosition = position;
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
        }
    }
}