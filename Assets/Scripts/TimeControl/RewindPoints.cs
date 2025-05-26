using System.Linq;
using UnityEngine;

public class RewindPoints : RewindAbstract
{
    PointsController pointsController;

    public struct PointEntryValues
    {
        public int totalPoints;
        public PointEntry[] pointEntrys;
        public float timeStored;
    }
    CircularArray<PointEntryValues> trackedPointEntries;

    protected void TrackPointEntries()
    {
        if (pointsController != null)
        {
            var currentTime = Time.time;
            PointEntryValues values = new PointEntryValues
            {
                pointEntrys = pointsController.PointEntries.ToArray(),
                totalPoints = pointsController.TotalPoints,
                timeStored = currentTime
            };
            trackedPointEntries.Write(values);
        }
    }
    protected void RestorePointEntries(float seconds)
    {
        PointEntryValues values = trackedPointEntries.GetValue(seconds);

        var currentTime = Time.time;
        foreach (var item in values.pointEntrys)
        {
            var difference = values.timeStored - item.TimeAdded;
            item.TimeAdded = currentTime + difference;
        }

        pointsController.SetPointEntries(values.pointEntrys.ToList(), values.totalPoints);
    }

    public override void Track()
    {
        if (pointsController != null)
        {
            TrackPointEntries();
        }
    }

    public override void Rewind(float seconds)
    {
        if (pointsController != null)
        {
            RestorePointEntries(seconds);
        }
    }

    private void Start()
    {
        pointsController = PointsController.instance;
        if (pointsController)
        {
            trackedPointEntries = new CircularArray<PointEntryValues>();
        }

        MainInit();
    }
}
