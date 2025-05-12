using UnityEngine;

public class AttackState : BaseState
{
    private float _moveTimer;
    private float _loosePlayerTimer;
    private float _shotTimer;

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Perform()
    {
        ProccessCanSeePlayer();
    }

    private void ProccessCanSeePlayer()
    {
        if (enemy.CanSeePlayer())
        {
            _loosePlayerTimer = 0;
            _moveTimer += Time.deltaTime;
            _shotTimer += Time.deltaTime;

            enemy.transform.LookAt(enemy.Player.transform);

            if (_shotTimer > enemy.fireRate)
            {
                Shoot();
            }

            if (_moveTimer > Random.Range(1, 5))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                _moveTimer = 0;
            }

            enemy.LastKnownPlayerPosition = enemy.Player.transform.position;
        }
        else
        {
            _loosePlayerTimer += Time.deltaTime;
            if (_loosePlayerTimer > 3)
            {
                if (enemy is StationaryEnemy)
                {
                    stateMachine.ChangeState(new StationaryState());
                }
                else
                {
                    stateMachine.ChangeState(new SearchState());
                }
            }
        }
    }

    public void Shoot()
    {
        Transform gunBarrel = enemy.gunBarrel;
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, gunBarrel.position, enemy.transform.rotation);
        Vector3 shootDirection = (enemy.Player.transform.position - gunBarrel.transform.position).normalized;
        Quaternion randomAngle = Quaternion.AngleAxis(Random.Range(-3f, 3f), Vector3.up);

        bullet.GetComponent<Rigidbody>().linearVelocity = randomAngle * shootDirection * 40;


        _shotTimer = 0;
    }
}
