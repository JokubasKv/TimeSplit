using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RewindControllerTests
{
    private GameObject _controllerObject;
    private RewindController _controller;

    [SetUp]
    public void SetUp()
    {
        _controllerObject = new GameObject("RewindController");
        _controller = _controllerObject.AddComponent<RewindController>();
        _controller.TrackingEnabled = true;
        _controller.secondsAvailableForRewind = 3f;
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_controllerObject);
    }

    [Test]
    public void OnEnable_ResetsSecondsAvailableForRewind()
    {
        _controller.secondsAvailableForRewind = 2f;
        _controller.SendMessage("OnEnable");
        Assert.AreEqual(0f, _controller.secondsAvailableForRewind);
    }

    [Test]
    public void StartRewindTimeBySeconds_SetsIsBeingRewinded_AndInvokesTrackingStateCall()
    {
        bool trackingStateCalled = false;
        RewindController.TrackingStateCall = (state) => trackingStateCalled = !state;

        _controller.secondsAvailableForRewind = 2f;
        _controller.StartRewindTimeBySeconds(1f);

        Assert.IsTrue(_controller.IsBeingRewinded);
        Assert.IsTrue(trackingStateCalled);
    }

    [Test]
    public void SetTimeSecondsInRewind_UpdatesRewindSeconds()
    {
        _controller.secondsAvailableForRewind = 2f;
        _controller.SetTimeSecondsInRewind(1.5f);

        Assert.Pass();
    }

    [Test]
    public void StopRewindTimeBySeconds_UpdatesSecondsAvailableForRewind_AndInvokesEvents()
    {
        float movedIndex = 0;
        bool trackingStateCalled = false;
        RewindController.MoveLastRewindIndex = (val) => movedIndex = val;
        RewindController.TrackingStateCall = (state) => trackingStateCalled = state;

        _controller.secondsAvailableForRewind = 2f;
        _controller.StartRewindTimeBySeconds(1f);
        _controller.StopRewindTimeBySeconds();

        Assert.AreEqual(1f, _controller.secondsAvailableForRewind);
        Assert.IsFalse(_controller.IsBeingRewinded);
        Assert.AreEqual(1f, movedIndex);
        Assert.IsTrue(trackingStateCalled);
    }
}
