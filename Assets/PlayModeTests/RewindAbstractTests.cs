using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class RewindAbstractTests
{
    private class TestRewind : RewindAbstract
    {
        public override void Track() { TrackTransform(); }
        public override void Rewind(float seconds) { RestoreTransform(seconds); }
        public void CallTrackObjectActiveState() => TrackObjectActiveState();
        public void CallRestoreObjectActiveState(float seconds) => RestoreObjectActiveState(seconds);
    }

    private GameObject testObj;
    private TestRewind rewind;

    [SetUp]
    public void SetUp()
    {
        testObj = new GameObject("TestObj");
        rewind = testObj.AddComponent<TestRewind>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testObj);
    }

    [Test]
    public void OnEnable_And_OnDisable_SubscribeAndUnsubscribe()
    {
        rewind.MainInit();
        testObj.SetActive(true);
        var result = (bool)typeof(RewindAbstract).GetField("isSubscribed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(rewind);
        Assert.IsTrue(result);
        testObj.SetActive(false);
        Assert.IsFalse((bool)typeof(RewindAbstract).GetField("isSubscribed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(rewind));
    }
}
