using UnityEngine;

public class StationaryState : BaseState
{
    private float _turnWitTimer;

    public override void Enter() { }

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
        if (_turnWitTimer <= 0)
        {
            _turnWitTimer = Random.Range(2f, 5f); // Random wait time between 2 to 5 seconds
        }

        _turnWitTimer -= Time.deltaTime;

        if (_turnWitTimer <= 0)
        {
            float randomAngle = Random.Range(0f, 360f);
            Quaternion targetRotation = Quaternion.Euler(0, randomAngle, 0);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 2f); // Smooth turning

            _turnWitTimer = Random.Range(2f, 5f);
        }
    }
}
