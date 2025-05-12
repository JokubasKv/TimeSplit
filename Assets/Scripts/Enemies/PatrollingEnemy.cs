public class PatrollingEnemy : Enemy
{
    void Start()
    {
        _stateMachine.Initialize(new PatrolState());
    }
}