using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTests
{
    private GameObject _enemyObject;
    private Enemy _enemy;
    private GameObject _playerObject;

    [SetUp]
    public void SetUp()
    {
        // Create player
        _playerObject = new GameObject("Player");
        _playerObject.tag = "Player";
        _playerObject.transform.position = Vector3.forward * 10;
        _playerObject.AddComponent<CharacterController>();

        // Create enemy
        _enemyObject = new GameObject("Enemy");
        _enemyObject.AddComponent<NavMeshAgent>();
        _enemyObject.AddComponent<StateMachine>();
        _enemyObject.AddComponent<EnemyHealth>();
        _enemy = _enemyObject.AddComponent<Enemy>();
        _enemy.sightDistance = 20f;
        _enemy.fieldOfView = 90f;
        _enemy.eyeHeight = 0f;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_enemyObject);
        Object.DestroyImmediate(_playerObject);
    }

    [Test]
    public void CanSeePlayer_PlayerInSight_ReturnsTrue()
    {
        _playerObject.transform.position = _enemyObject.transform.position + _enemyObject.transform.forward * 10;
        Assert.IsTrue(_enemy.CanSeePlayer());
    }

    [Test]
    public void CanSeePlayer_PlayerOutOfSightDistance_ReturnsFalse()
    {
        _playerObject.transform.position = _enemyObject.transform.position + _enemyObject.transform.forward * 100;
        Assert.IsFalse(_enemy.CanSeePlayer());
    }

    [Test]
    public void CanSeePlayer_PlayerBehindEnemy_ReturnsFalse()
    {
        _playerObject.transform.position = _enemyObject.transform.position - _enemyObject.transform.forward * 10;
        Assert.IsFalse(_enemy.CanSeePlayer());
    }

    [Test]
    public void CanSeePlayer_NoPlayer_ReturnsFalse()
    {
        Object.DestroyImmediate(_playerObject);
        Assert.IsFalse(_enemy.CanSeePlayer());
    }

    [Test]
    public void LastKnownPlayerPosition_Property_SetAndGet()
    {
        Vector3 pos = new Vector3(1, 2, 3);
        _enemy.LastKnownPlayerPosition = pos;
        Assert.AreEqual(pos, _enemy.LastKnownPlayerPosition);
    }

    [Test]
    public void Agent_Property_ReturnsNavMeshAgent()
    {
        Assert.IsNotNull(_enemy.Agent);
        Assert.IsInstanceOf<NavMeshAgent>(_enemy.Agent);
    }

    [Test]
    public void Health_Property_ReturnsEnemyHealth()
    {
        Assert.IsNotNull(_enemy.Health);
        Assert.IsInstanceOf<EnemyHealth>(_enemy.Health);
    }

    [Test]
    public void Player_Property_ReturnsPlayerGameObject()
    {
        Assert.IsNotNull(_enemy.Player);
        Assert.AreEqual(_playerObject, _enemy.Player);
    }
}
