using UnityEngine;

public class TimeClockHandController : MonoBehaviour
{
    [Header("Required Inheritance")]
    public Transform clockHand;

    private Vector3 _startingRotation;

    private void Start()
    {
        _startingRotation = clockHand.localEulerAngles;
    }

    public void SetClockHandRotation(float percentage)
    {
        percentage = Mathf.Clamp01(percentage);
        float angle = 360f * percentage;
        clockHand.localEulerAngles = new Vector3(_startingRotation.x, _startingRotation.y, _startingRotation.z + angle);
    }
}
