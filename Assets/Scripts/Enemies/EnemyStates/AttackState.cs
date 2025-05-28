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
        if (RewindController.instance.IsBeingRewinded)
        {
            stateMachine.ChangeState(new CloseAttackState());
            return;
        }

        ProccessCanSeePlayer();
    }

    private void ProccessCanSeePlayer()
    {
        if (enemy.CanSeePlayer())
        {
            _loosePlayerTimer = 0;
            _moveTimer += Time.deltaTime;
            _shotTimer += Time.deltaTime;

            enemy.LookAt(enemy.Player.transform.position);

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
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/EnemyBullet") as GameObject, gunBarrel.position, enemy.transform.rotation);
        Vector3 shootDirection = (enemy.Player.transform.position - gunBarrel.transform.position).normalized;
        Quaternion randomAngle = Quaternion.AngleAxis(Random.Range(-3f, 3f), Vector3.up);

        // Start coroutine to grow and then shoot the bullet
        enemy.StartCoroutine(GrowAndShootBullet(bullet, randomAngle * shootDirection * 40));

        _shotTimer = 0;
    }

    private System.Collections.IEnumerator GrowAndShootBullet(GameObject bullet, Vector3 velocity)
    {
        float growDuration = 1f;
        float timer = 0f;
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = bullet.transform.localScale;
        bullet.transform.localScale = initialScale;
        bullet.transform.SetParent(enemy.gunBarrel.transform);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        while (timer < growDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / growDuration);
            bullet.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;
        }

        if (rb != null)
        {
            bullet.transform.SetParent(null);
            rb.isKinematic = false;
            rb.linearVelocity = velocity;
        }
    }
}
