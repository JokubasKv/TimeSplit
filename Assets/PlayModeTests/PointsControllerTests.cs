using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PointsControllerTests
{
    private class DummyUIManager : UIManager
    {
        public List<string> LastPointEntryTexts;
        public int LastTotalPoints;

        // Use 'new' to explicitly hide the inherited member and avoid CS0108 warning
        public new void SetPointEntryText(List<string> pointEntryTexts, int totalPoints)
        {
            LastPointEntryTexts = pointEntryTexts;
            LastTotalPoints = totalPoints;
        }
    }

    private PointsController pointsController;
    private DummyUIManager dummyUIManager;

    [SetUp]
    public void SetUp()
    {
        var go = new GameObject("PointsController");
        pointsController = go.AddComponent<PointsController>();

        var uiGo = new GameObject("UIManager");
        dummyUIManager = uiGo.AddComponent<DummyUIManager>();

        // Create separate GameObjects for each Graphic type to avoid Unity's single-Graphic-per-GameObject restriction
        var promptGo = new GameObject("PromptText");
        promptGo.transform.SetParent(uiGo.transform);
        var promptText = promptGo.AddComponent<TextMeshProUGUI>();
        dummyUIManager.promptText = promptText;

        var hurtGo = new GameObject("HurtImage");
        hurtGo.transform.SetParent(uiGo.transform);
        var hurtImage = hurtGo.AddComponent<Image>();
        dummyUIManager.hurtImage = hurtImage;

        var pointsGo = new GameObject("PointsText");
        pointsGo.transform.SetParent(uiGo.transform);
        var pointsText = pointsGo.AddComponent<TextMeshProUGUI>();
        dummyUIManager.pointsText = pointsText;

        var entryGo = new GameObject("PointEntryText");
        entryGo.transform.SetParent(uiGo.transform);
        var entryText = entryGo.AddComponent<TextMeshProUGUI>();
        dummyUIManager.pointEntryText = entryText;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(pointsController.gameObject);
        Object.DestroyImmediate(dummyUIManager.gameObject);
    }

    [Test]
    public void AddPoints_AddsEntryAndUpdatesTotal()
    {
        pointsController.AddPoints("TestSource", 42);
        Assert.AreEqual(42, pointsController.TotalPoints);
        Assert.AreEqual(1, pointsController.PointEntries.Count);
        Assert.AreEqual("TestSource", pointsController.PointEntries[0].SourceText);
        Assert.AreEqual(42, pointsController.PointEntries[0].Points);
    }

    [Test]
    public void AddEnemyKillPoints_RobotBullet_AddsCorrectPoints()
    {
        pointsController.AddEnemyKillPoints(EnemyType.Robot, DamageMethod.Bullet);
        Assert.AreEqual(100, pointsController.TotalPoints);
        Assert.AreEqual("Robot (Bullet)", pointsController.PointEntries[0].SourceText);
    }

    [Test]
    public void AddEnemyKillPoints_TurretThrownObject_AccumulatesPoints()
    {
        pointsController.AddEnemyKillPoints(EnemyType.Turret, DamageMethod.ThrownObject);
        pointsController.AddEnemyKillPoints(EnemyType.Turret, DamageMethod.ThrownObject_Stool);
        Assert.AreEqual(120 + 160, pointsController.TotalPoints);
        Assert.AreEqual(2, pointsController.PointEntries.Count);
    }

    [Test]
    public void ResetPoints_ClearsEntriesAndTotal()
    {
        pointsController.AddPoints("A", 1);
        pointsController.AddPoints("B", 2);
        pointsController.ResetPoints();
        Assert.AreEqual(0, pointsController.TotalPoints);
        Assert.AreEqual(0, pointsController.PointEntries.Count);
    }

    [Test]
    public void SetPointEntries_SetsEntriesAndTotal()
    {
        var entries = new List<PointEntry>
       {
           new PointEntry("A", 5, 3f),
           new PointEntry("B", 10, 3f)
       };
        pointsController.SetPointEntries(entries, 15);
        Assert.AreEqual(2, pointsController.PointEntries.Count);
        Assert.AreEqual(15, pointsController.TotalPoints);
    }
}
