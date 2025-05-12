using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum OutOfBoundsBehaviour
{
    Disable,
    DisableDestroy
}

public class RewindManager : MonoSingleton<RewindManager>
{
    public static readonly float secondsToTrack = 6;
    public float secondsAvailableForRewind;
    public bool IsBeingRewinded { get; private set; } = false;
    public bool TrackingEnabled { get; set; } = true;


    float rewindSeconds = 0;
    List<RewindAbstract> _rewindedObjects;
    List<(RewindAbstract obj, OutOfBoundsBehaviour outOfBoundsBehaviour)> _lateAddedRewindedObjects = new List<(RewindAbstract obj, OutOfBoundsBehaviour outOfBoundsBehaviour)>();


    [SerializeField] public static Action<float> RewindTimeCall;
    [SerializeField] public static Action<bool> TrackingStateCall;
    [SerializeField] public static Action<float> RestoreBuffers;

    public void AddObjectForTracking(RewindAbstract objectToRewind, OutOfBoundsBehaviour outOfBoundsBehaviour)
    {
        if (!_lateAddedRewindedObjects.Any(x => x.Item1 == objectToRewind))
        {
            objectToRewind.MainInit();
            _lateAddedRewindedObjects.Add(new(objectToRewind, outOfBoundsBehaviour));
        }
    }

    private void OnEnable()
    {
        secondsAvailableForRewind = 0;
    }

    public void StartRewindTimeBySeconds(float seconds)
    {
        CheckReachingOutOfBounds(seconds);

        rewindSeconds = seconds;
        TrackingStateCall?.Invoke(false);
        IsBeingRewinded = true;
    }


    public void SetTimeSecondsInRewind(float seconds)
    {
        CheckReachingOutOfBounds(seconds);
        rewindSeconds = seconds;
    }

    public void StopRewindTimeBySeconds()
    {
        secondsAvailableForRewind -= rewindSeconds;
        IsBeingRewinded = false;
        RestoreBuffers?.Invoke(rewindSeconds);
        TrackingStateCall?.Invoke(true);
    }
    private void CheckReachingOutOfBounds(float seconds)
    {
        if (seconds > secondsAvailableForRewind)
        {
            Debug.LogError("Not enough stored tracked value!");
            return;
        }
    }

    private void FixedUpdate()
    {
        if (IsBeingRewinded)
        {
            UIManager.instance.SetTimeBar((secondsAvailableForRewind - rewindSeconds) / (float)secondsToTrack);
            RewindTimeCall?.Invoke(rewindSeconds);
        }
        else if (secondsAvailableForRewind != secondsToTrack)
        {
            UIManager.instance.SetTimeBar(secondsAvailableForRewind / (float)secondsToTrack);
            secondsAvailableForRewind += Time.fixedDeltaTime;

            if (secondsAvailableForRewind > secondsToTrack)
                secondsAvailableForRewind = secondsToTrack;
        }
    }

    protected override void InternalInit()
    {
        _rewindedObjects = FindObjectsByType<RewindAbstract>(FindObjectsSortMode.None).ToList();

        _rewindedObjects.ForEach(x => x.MainInit());
    }

    protected override void InternalOnDestroy()
    {
    }
}
