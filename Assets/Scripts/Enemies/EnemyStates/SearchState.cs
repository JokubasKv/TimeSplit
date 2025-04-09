using UnityEngine;

public class SearchState : BaseState
{
    //Timers
    private float _searchTimer;
    private float _searchMoveTimer;

    public override void Enter()
    {
        enemy.Agent.SetDestination(enemy.LastKnownPlayerPosition);
    }

    public override void Exit()
    {

    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(new AttackState());
        }

        if (enemy.Agent.remainingDistance < enemy.Agent.stoppingDistance)
        {
            _searchTimer += Time.deltaTime;
            _searchMoveTimer += Time.deltaTime;

            if (_searchMoveTimer > Random.Range(1, 5))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                _searchMoveTimer = 0;
            }
            if (_searchTimer > 5)
            {
                stateMachine.ChangeState(new PatrolState());
            }
        }

    }
}
