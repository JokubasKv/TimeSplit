using UnityEngine;

public class RewindPosition : RewindAbstract
{
    [SerializeField] bool trackTransform;

    public override void Rewind(float seconds)
    {
        if (trackTransform)
        {
            RestoreTransform(seconds);
        }
    }

    public override void Track()
    {
        if (trackTransform)
        {
            TrackTransform();
        }
    }

    private void Start()
    {
        MainInit();
    }
}
