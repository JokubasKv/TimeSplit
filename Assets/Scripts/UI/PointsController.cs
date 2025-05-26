using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointEntry
{
    public string SourceText;
    public int Points;
    public float TimeAdded;
    public float Duration;

    public PointEntry(string sourceText, int points, float duration)
    {
        SourceText = sourceText;
        Points = points;
        TimeAdded = Time.time;
        Duration = duration;
    }

    public bool IsExpired => Time.time - TimeAdded > Duration;
}

public class PointsController : MonoSingleton<PointsController>
{

    private List<PointEntry> pointEntries = new List<PointEntry>();
    public List<PointEntry> PointEntries => pointEntries;

    [SerializeField] private float displayDuration = 3f;

    public int TotalPoints { get; private set; }

    public void AddPoints(string sourceText, int points)
    {
        TotalPoints += points;
        pointEntries.Add(new PointEntry(sourceText, points, displayDuration));

        SetUIPointEntryText();
    }

    public void AddEnemyKillPoints(EnemyType enemyType, DamageMethod damageMethod)
    {
        int points = 0;
        string sourceText = "";

        switch (enemyType)
        {
            case EnemyType.Robot:
                switch (damageMethod)
                {
                    case DamageMethod.Bullet:
                        points = 100;
                        sourceText = "Robot (Bullet)";
                        break;
                    case DamageMethod.ThrownObject:
                        points = 150;
                        sourceText = "Robot (Thrown Object)";
                        break;
                    case DamageMethod.ThrownObject_Stool:
                        points = 200;
                        sourceText = "Robot (Stool)";
                        break;
                    default:
                        points = 50;
                        sourceText = "Robot (Unknown)";
                        break;
                }
                break;
            case EnemyType.Turret:
                switch (damageMethod)
                {
                    case DamageMethod.Bullet:
                        points = 80;
                        sourceText = "Turret (Bullet)";
                        break;
                    case DamageMethod.ThrownObject:
                        points = 120;
                        sourceText = "Turret (Thrown Object)";
                        break;
                    case DamageMethod.ThrownObject_Stool:
                        points = 160;
                        sourceText = "Turret (Stool)";
                        break;
                    default:
                        points = 40;
                        sourceText = "Turret (Unknown)";
                        break;
                }
                break;
            default:
                points = 10;
                sourceText = "Unknown Enemy";
                break;
        }

        AddPoints(sourceText, points);
    }

    void Update()
    {
        if (RewindController.instance.IsBeingRewinded)
        {
            return;
        }

        RemoveOldPointEntries();

    }

    private void RemoveOldPointEntries()
    {
        var pointEntriesRemoved = false;

        for (int i = pointEntries.Count - 1; i >= 0; i--)
        {
            if (pointEntries[i].IsExpired)
            {
                pointEntries.RemoveAt(i);
                pointEntriesRemoved = true;
            }
        }

        if (pointEntriesRemoved)
        {
            SetUIPointEntryText();
        }
    }

    public void SetPointEntries(List<PointEntry> entries, int totalPoints)
    {
        pointEntries = entries;
        TotalPoints = totalPoints;
        SetUIPointEntryText();
    }

    private List<string> GetPointEntriesTexts()
    {
        return pointEntries.Select(entry => $"{entry.SourceText}: {entry.Points}").ToList();
    }

    public void ResetPoints()
    {
        pointEntries.Clear();
        TotalPoints = 0;
        SetUIPointEntryText();
    }

    private void SetUIPointEntryText()
    {
        var pointEntryTexts = GetPointEntriesTexts();
        UIManager.instance.SetPointEntryText(pointEntryTexts, TotalPoints);
    }

    protected override void InternalInit()
    {
    }

    protected override void InternalOnDestroy()
    {
    }
}
