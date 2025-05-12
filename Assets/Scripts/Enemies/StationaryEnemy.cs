public class StationaryEnemy : Enemy
{
    void Start()
    {
        _stateMachine.Initialize(new StationaryState());
    }
}