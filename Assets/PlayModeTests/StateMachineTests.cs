using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StateMachineTests
{
    private GameObject _gameObject;
    private StateMachine _stateMachine;
    private TestState _testState;
    private Enemy _mockEnemy;
    private RewindAbstract _mockRewindAbstract;
    private RewindController _rewindController;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject();
        _stateMachine = _gameObject.AddComponent<StateMachine>();

        // Mock Enemy
        _mockEnemy = _gameObject.AddComponent<Enemy>();

        // Mock RewindAbstract
        _mockRewindAbstract = _gameObject.AddComponent<TestRewindAbstract>();

        // Mock RewindController singleton
        _rewindController = _gameObject.AddComponent<RewindController>();

        // TestState
        _testState = new TestState();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void Initialize_SetsRewindReferences_AndChangesState()
    {
        _stateMachine.Initialize(_testState);

        Assert.AreEqual(_testState, _stateMachine.activeState);
        Assert.IsTrue(_testState.EnterCalled);
    }

    [Test]
    public void ChangeState_ExitsPreviousState_AndEntersNewState()
    {
        var prevState = new TestState();
        _stateMachine.activeState = prevState;

        _stateMachine.ChangeState(_testState);

        Assert.IsTrue(prevState.ExitCalled);
        Assert.IsTrue(_testState.EnterCalled);
        Assert.AreEqual(_testState, _stateMachine.activeState);
    }

    [Test]
    public void Update_PerformsActiveState_WhenNotRewinding()
    {
        _stateMachine.activeState = _testState;
        typeof(StateMachine)
            .GetField("_rewindManager", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(_stateMachine, _rewindController);
        typeof(StateMachine)
            .GetField("_rewindAbstract", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(_stateMachine, null);

        _rewindController.StopRewindTimeBySeconds();

        _stateMachine.SendMessage("Update");

        Assert.IsTrue(_testState.PerformCalled);
    }

    [Test]
    public void Update_DoesNotPerform_WhenRewinding()
    {
        _stateMachine.activeState = _testState;
        typeof(StateMachine)
            .GetField("_rewindManager", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(_stateMachine, _rewindController);
        typeof(StateMachine)
            .GetField("_rewindAbstract", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(_stateMachine, _mockRewindAbstract);

        _rewindController.StartRewindTimeBySeconds(-1);

        _stateMachine.SendMessage("Update");

        Assert.IsFalse(_testState.PerformCalled);
    }

    // Test double for BaseState
    private class TestState : BaseState
    {
        public bool EnterCalled { get; private set; }
        public bool ExitCalled { get; private set; }
        public bool PerformCalled { get; private set; }

        public override void Enter() => EnterCalled = true;
        public override void Exit() => ExitCalled = true;
        public override void Perform() => PerformCalled = true;
    }

    // Test double for RewindAbstract
    private class TestRewindAbstract : RewindAbstract
    {
        public override void Track() { }
        public override void Rewind(float seconds) { }
    }

}
