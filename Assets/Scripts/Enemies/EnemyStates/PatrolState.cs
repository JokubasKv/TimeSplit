using UnityEngine;

public class PatrolState : BaseState
{
    public int waypointIndex;
    public float waypointWaitTime = 3f;
    private float _waitTimer;

    public override void Enter()
    {
        if (enemy.path == null || enemy.path.waypoints == null || enemy.path.waypoints.Count == 0)
        {
            Vector3 randomDirection = enemy.transform.position + new Vector3(
                Random.Range(-10f, 10f),
                0,
                Random.Range(-10f, 10f)
            );
            enemy.Agent.SetDestination(randomDirection);
        }
        else
        {
            enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
        }
    }

    public override void Exit()
    {

    }

    public override void Perform()
    {
        PatrolCycle();
        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(new AttackState());
        }
    }

    public void PatrolCycle()
    {
        if (enemy.Agent.remainingDistance < 0.2f)
        {

            _waitTimer += Time.deltaTime;
            if (_waitTimer > waypointWaitTime)
            {
                if (enemy.path == null || enemy.path.waypoints == null || enemy.path.waypoints.Count == 0)
                {
                    Vector3 randomDirection = enemy.transform.position + new Vector3(
                        Random.Range(-10f, 10f),
                        0,
                        Random.Range(-10f, 10f)
                    );
                    enemy.Agent.SetDestination(randomDirection);
                }
                else
                {
                    if (waypointIndex < enemy.path.waypoints.Count - 1)
                    {
                        waypointIndex++;
                    }
                    else
                    {
                        waypointIndex = 0;
                    }
                    enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);

                }
                _waitTimer = 0;
            }
        }
    }
}
