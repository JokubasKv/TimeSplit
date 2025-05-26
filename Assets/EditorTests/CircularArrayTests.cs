using NUnit.Framework;
using UnityEngine;

public class CircularArrayTests
{
    [Test]
    public void CircularArray_Should_ReturnLastValue()
    {
        var circularArray = new CircularArray<int>(1);
        var howManyRecords = (int)(Time.timeScale / Time.fixedDeltaTime) * 1;

        for (int i = 0; i < howManyRecords; i++)
        {
            circularArray.Write(i);
        }

        var result = circularArray.GetLastValue();

        Assert.AreEqual(howManyRecords - 1, result);
    }

    [Test]
    public void CircularArray_Should_OverwriteOldValues_WhenFull()
    {
        var circularArray = new CircularArray<int>(1);
        var howManyRecords = (int)(Time.timeScale / Time.fixedDeltaTime) * 1;

        for (int i = 0; i < howManyRecords; i++)
        {
            circularArray.Write(i);
        }

        circularArray.Write(100);

        var lastValue = circularArray.GetLastValue();
        Assert.AreEqual(100, lastValue);
    }

    [Test]
    public void CircularArray_Should_ReturnCorrectValue_ForGivenSeconds()
    {
        var circularArray = new CircularArray<int>(1);
        var howManyRecords = (int)(Time.timeScale / Time.fixedDeltaTime) * 1;

        for (int i = 0; i < howManyRecords; i++)
        {
            circularArray.Write(i);
        }

        var result = circularArray.GetValue(Time.fixedDeltaTime * 2);

        Assert.AreEqual(howManyRecords - 2, result);
    }

    [Test]
    public void CircularArray_Should_HandleMovingOfLastIndex()
    {
        var circularArray = new CircularArray<int>(1);
        var howManyRecords = (int)(Time.timeScale / Time.fixedDeltaTime) * 1;

        for (int i = 0; i < howManyRecords; i++)
        {
            circularArray.Write(i);
        }

        RewindController.MoveLastRewindIndex.Invoke(Time.fixedDeltaTime * 2);

        var result = circularArray.GetLastValue();
        Assert.AreEqual(howManyRecords - 2, result);
    }
}
