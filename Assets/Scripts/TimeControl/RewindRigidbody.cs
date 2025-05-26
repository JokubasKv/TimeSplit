using UnityEngine;

public class RewindRigidbody : RewindAbstract
{
    Rigidbody rigidBody;

    [SerializeField] bool trackActiveState;

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
            trackedVelocities.Write(values);
        }
    }
    protected void RestoreVelocity(float seconds)
    {
        VelocityValues values = trackedVelocities.GetValue(seconds);
        rigidBody.linearVelocity = values.velocity;
        rigidBody.angularVelocity = values.angularVelocity;
    }


    public override void Rewind(float seconds)
    {
        RestoreTransform(seconds);
        RestoreVelocity(seconds);

        if (trackActiveState)
        {
            RestoreObjectActiveState(seconds);
        }
    }

    public override void Track()
    {
        TrackTransform();
        TrackVelocity();

        if (trackActiveState)
        {
            TrackObjectActiveState();
        }
    }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        if (rigidBody != null)
        {
            trackedVelocities = new CircularArray<VelocityValues>();
        }

        MainInit();
    }
}
