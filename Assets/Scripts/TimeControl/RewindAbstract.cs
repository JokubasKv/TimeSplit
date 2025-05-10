using System;
using UnityEngine;

public abstract class RewindAbstract : MonoBehaviour
{
    RewindManager rewindManager;
    public bool IsTracking { get; set; } = false;

    Rigidbody rigidBody;
    AudioSource audioSource;

    public void MainInit()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        trackedActiveStates = new CircularArray<bool>();
        trackedTransformValues = new CircularArray<TransformValues>();

        if (rigidBody != null)
        {
            trackedVelocities = new CircularArray<VelocityValues>();
        }

        if (audioSource != null)
        {
            trackedAudioTimes = new CircularArray<AudioTrackedData>();
        }

        IsTracking = true;
    }

    protected void FixedUpdate()
    {
        if (IsTracking)
            Track();
    }

    #region ActiveState
    public bool IsManagerTrackingActiveState { get; set; } = false;
    public bool IsActiveStateTracked { get; set; } = false;

    CircularArray<bool> trackedActiveStates;

    public void TrackObjectActiveState()
    {
        IsActiveStateTracked = true;
        trackedActiveStates.WriteLastValue(gameObject.activeSelf);
    }

    public void RestoreObjectActiveState(float seconds)
    {
        gameObject.SetActive(trackedActiveStates.ReadFromBuffer(seconds));
    }

    #endregion


    #region Transform

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
        trackedTransformValues.WriteLastValue(valuesToWrite);
        Debug.Log(valuesToWrite.ToString());
    }

    protected void RestoreTransform(float seconds)
    {
        TransformValues valuesToRead = trackedTransformValues.ReadFromBuffer(seconds);
        transform.SetPositionAndRotation(valuesToRead.position, valuesToRead.rotation);
        transform.localScale = valuesToRead.scale;
    }
    #endregion

    #region Velocity
    public struct VelocityValues
    {
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }
    CircularArray<VelocityValues> trackedVelocities;

    protected void TrackVelocity()
    {
        if (rigidBody != null)
        {
            VelocityValues values;
            values.velocity = rigidBody.linearVelocity;
            values.angularVelocity = rigidBody.angularVelocity;
            trackedVelocities.WriteLastValue(values);
        }
    }
    protected void RestoreVelocity(float seconds)
    {
        VelocityValues values = trackedVelocities.ReadFromBuffer(seconds);
        rigidBody.linearVelocity = values.velocity;
        rigidBody.angularVelocity = values.angularVelocity;
    }
    #endregion

    #region Audio
    CircularArray<AudioTrackedData> trackedAudioTimes;
    public struct AudioTrackedData
    {
        public float time;
        public bool isPlaying;
        public bool isEnabled;
    }
    protected void TrackAudio()
    {
        if (audioSource == null)
        {
            Debug.LogError("Cannot find AudioSource on the object, while TrackAudio() is being called!!!");
            return;
        }

        audioSource.volume = 1;
        AudioTrackedData dataToWrite;
        dataToWrite.time = audioSource.time;
        dataToWrite.isEnabled = audioSource.enabled;
        dataToWrite.isPlaying = audioSource.isPlaying;

        trackedAudioTimes.WriteLastValue(dataToWrite);
    }
    protected void RestoreAudio(float seconds)
    {
        AudioTrackedData readValues = trackedAudioTimes.ReadFromBuffer(seconds);
        audioSource.enabled = readValues.isEnabled;
        if (readValues.isPlaying)
        {
            audioSource.time = readValues.time;
            audioSource.volume = 0;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    #endregion

    private void OnTrackingChange(bool val)
    {
        IsTracking = val;
    }
    protected void OnEnable()
    {
        RewindManager.RewindTimeCall += Rewind;
        RewindManager.TrackingStateCall += OnTrackingChange;
    }
    protected void OnDisable()
    {
        RewindManager.RewindTimeCall -= Rewind;
        RewindManager.TrackingStateCall -= OnTrackingChange;
    }


    public abstract void Track();
    public abstract void Rewind(float seconds);

}
