using UnityEngine;

public class StationaryState : BaseState
{
    private float _lookInterval = 3f;
    private float _lookTimer = 0f;
    private Quaternion _targetRotation;

    public override void Enter()
    {
        _targetRotation = enemy.transform.rotation;
    }

    public override void Exit() { }

    public override void Perform()
    {
        StationaryPatrolCycle();
        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(new AttackState());
        }
    }

    public void StationaryPatrolCycle()
    {
        _lookTimer += Time.deltaTime;
        if (_lookTimer >= _lookInterval)
        {
            _lookTimer = 0f;
            float angle = Random.Range(0f, 360f);
            Vector3 direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
            Vector3 lookPosition = enemy.transform.position + direction;
            enemy.LookAt(lookPosition);
        }
    }
}
