using NUnit.Framework;
using UnityEngine;

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
    public void MainInit_InitializesFields()
    {
        testObj.AddComponent<Rigidbody>();
        rewind.MainInit();
        Assert.IsTrue(rewind.IsTracking);
    }

    [Test]
    public void TrackTransform_And_RestoreTransform_Works()
    {
        rewind.MainInit();
        testObj.transform.position = new Vector3(1, 2, 3);
        testObj.transform.rotation = Quaternion.Euler(10, 20, 30);
        testObj.transform.localScale = new Vector3(2, 2, 2);
        rewind.Track();
        testObj.transform.position = Vector3.zero;
        testObj.transform.rotation = Quaternion.identity;
        testObj.transform.localScale = Vector3.one;
        rewind.Rewind(0);
        Assert.AreEqual(new Vector3(1, 2, 3), testObj.transform.position);
        Assert.True(Quaternion.Euler(10, 20, 30).eulerAngles.FuzzyEquals(testObj.transform.rotation.eulerAngles, 0.1f));
        Assert.AreEqual(new Vector3(2, 2, 2), testObj.transform.localScale);
    }


    [Test]
    public void TrackObjectActiveState_And_RestoreObjectActiveState_Works()
    {
        rewind.MainInit();
        testObj.SetActive(false);
        rewind.CallTrackObjectActiveState();
        testObj.SetActive(true);
        rewind.CallRestoreObjectActiveState(0);
        Assert.IsFalse(testObj.activeSelf);
    }
}
