using System;
using System.Linq;
using UnityEngine;

public class RewindController : MonoSingleton<RewindController>
{
    public static readonly float secondsToTrack = 6;
    public float secondsAvailableForRewind;
    public bool IsBeingRewinded { get; private set; } = false;
    public bool TrackingEnabled { get; set; } = true;


    float rewindSeconds = 0;

    [SerializeField] public static Action<float> RewindTimeCall;
    [SerializeField] public static Action<bool> TrackingStateCall;
    [SerializeField] public static Action<float> MoveLastRewindIndex;
    [SerializeField] public static Action ActivateStateUpdateCall;

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
        MoveLastRewindIndex?.Invoke(rewindSeconds);
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
            if (UIManager.instance != null)
            {
                UIManager.instance.SetTimeBar((secondsAvailableForRewind - rewindSeconds) / (float)secondsToTrack);
            }
            RewindTimeCall?.Invoke(rewindSeconds);
        }
        else if (secondsAvailableForRewind != secondsToTrack)
        {
            if (UIManager.instance != null)
            {
                UIManager.instance.SetTimeBar(secondsAvailableForRewind / (float)secondsToTrack);
            }
            secondsAvailableForRewind += Time.fixedDeltaTime;

            if (secondsAvailableForRewind > secondsToTrack)
                secondsAvailableForRewind = secondsToTrack;
        }

        ActivateStateUpdateCall?.Invoke();
    }

    protected override void InternalInit()
    {
        var existingRewindsObjects = FindObjectsByType<RewindAbstract>(FindObjectsSortMode.None).ToList();

        existingRewindsObjects.ForEach(x => x.MainInit());
    }

    protected override void InternalOnDestroy()
    {
    }
}
