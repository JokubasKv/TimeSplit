using UnityEngine;

public abstract class RewindAbstract : MonoBehaviour
{
    public bool IsTracking { get; set; } = false;

    public void MainInit()
    {
        trackedActiveStates = new CircularArray<bool>();
        trackedTransformValues = new CircularArray<TransformValues>();

        IsTracking = true;
    }

    protected void FixedUpdate()
    {
        if (IsTracking)
            Track();
    }

    public bool istrackingActiveState { get; set; } = false;

    CircularArray<bool> trackedActiveStates;

    public void TrackObjectActiveState()
    {
        istrackingActiveState = true;
        trackedActiveStates.Write(gameObject.activeSelf);
    }

    public void RestoreObjectActiveState(float seconds)
    {
        gameObject.SetActive(trackedActiveStates.GetValue(seconds));
    }

    CircularArray<TransformValues> trackedTransformValues;
    public struct TransformValues
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    protected void TrackTransform()
    {
        TransformValues valuesToWrite;
        valuesToWrite.position = transform.position;
        valuesToWrite.rotation = transform.rotation;
        valuesToWrite.scale = transform.localScale;
        trackedTransformValues.Write(valuesToWrite);
    }

    protected void RestoreTransform(float seconds)
    {
        TransformValues valuesToRead = trackedTransformValues.GetValue(seconds);
        transform.SetPositionAndRotation(valuesToRead.position, valuesToRead.rotation);
        transform.localScale = valuesToRead.scale;
    }

    private void OnTrackingChange(bool val)
    {
        IsTracking = val;
    }

    private bool isSubscribed = false;

    protected void OnEnable()
    {
        if (!isSubscribed)
        {
            RewindController.RewindTimeCall += Rewind;
            RewindController.TrackingStateCall += OnTrackingChange;
            isSubscribed = true;
        }
    }

    protected void OnDisable()
    {
        if (isSubscribed && !istrackingActiveState)
        {
            RewindController.RewindTimeCall -= Rewind;
            RewindController.TrackingStateCall -= OnTrackingChange;
            isSubscribed = false;
        }
    }

    protected void OnDestroy()
    {
        if (isSubscribed)
        {
            RewindController.RewindTimeCall -= Rewind;
            RewindController.TrackingStateCall -= OnTrackingChange;
            isSubscribed = false;
        }
    }

    public abstract void Track();
    public abstract void Rewind(float seconds);

}
