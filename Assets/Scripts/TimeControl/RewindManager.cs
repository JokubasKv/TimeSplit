using System;
using UnityEngine;

public class RewindManager : MonoSingleton<RewindManager>
{
    [SerializeField] public static Action<float> RewindTimeCall;
    [SerializeField] public static Action<bool> TrackingStateCall;
    [SerializeField] public static Action<float> RestoreBuffers;

    float rewindSeconds = 0;

    public static readonly float secondsToTrack = 6;
    public float secondsAvailableForRewind;
    public bool IsBeingRewinded = false;

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
    private void OnEnable()
    {
        secondsAvailableForRewind = 0;
    }

    private void FixedUpdate()
    {

        if (IsBeingRewinded)
        {
            UIManager.instance.SetTimeBar(secondsAvailableForRewind - rewindSeconds / (float)secondsAvailableForRewind);
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
    }

    protected override void InternalOnDestroy()
    {
    }
}
