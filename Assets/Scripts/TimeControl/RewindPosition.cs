using UnityEngine;

public class RewindPosition : RewindAbstract
{
    [SerializeField] bool trackActiveState;

    public override void Rewind(float seconds)
    {
        RestoreTransform(seconds);

        if (trackActiveState)
        {
            RestoreObjectActiveState(seconds);
        }
    }

    public override void Track()
    {
        TrackTransform();

        if (trackActiveState)
        {
            TrackObjectActiveState();
        }
    }

    private void Start()
    {
        MainInit();
    }
}
