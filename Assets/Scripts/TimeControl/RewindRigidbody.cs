public class RewindRigidbody : RewindAbstract
{
    public override void Rewind(float seconds)
    {
        RestoreTransform(seconds);
        RestoreVelocity(seconds);
    }

    public override void Track()
    {
        TrackTransform();
        TrackVelocity();
    }

    private void Start()
    {
        MainInit();
    }
}
