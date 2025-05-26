using UnityEngine;

public class CircularArray<T>
{
    T[] data;
    int currentIndex = -1;
    int arrayCapacity;
    float howManyRecordsPerSecond;

    public CircularArray() : this((int)RewindController.secondsToTrack)
    {
    }

    public CircularArray(int secondsToTrack)
    {
        howManyRecordsPerSecond = Time.timeScale / Time.fixedDeltaTime;
        arrayCapacity = (int)(secondsToTrack * howManyRecordsPerSecond);
        data = new T[arrayCapacity];
        RewindController.MoveLastRewindIndex += OnMoveCurrentIndex;
    }

    public void Write(T value)
    {
        currentIndex++;
        if (currentIndex >= arrayCapacity)
        {
            currentIndex = 0;
            data[currentIndex] = value;
        }
        else
        {
            data[currentIndex] = value;
        }
    }

    public T GetLastValue()
    {
        return data[currentIndex];
    }

    public T GetValue(float seconds)
    {
        int indexOffset = (int)(howManyRecordsPerSecond * seconds);

        if ((currentIndex - indexOffset) < 0)
        {
            int wrappedIndex = arrayCapacity - (indexOffset - currentIndex);
            return data[wrappedIndex];
        }
        else
        {
            return data[currentIndex - indexOffset];
        }
    }
    private void MoveCurrentIndex(float seconds)
    {
        int indexOffset = (int)(howManyRecordsPerSecond * seconds);

        if ((currentIndex - indexOffset) < 0)
        {
            currentIndex = arrayCapacity - (indexOffset - currentIndex);
        }
        else
        {
            currentIndex -= indexOffset;
        }
    }
    private void OnMoveCurrentIndex(float seconds)
    {
        MoveCurrentIndex(seconds);
    }
}