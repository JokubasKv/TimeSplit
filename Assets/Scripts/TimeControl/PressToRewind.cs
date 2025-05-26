using UnityEngine;
using UnityEngine.InputSystem;

public class PressToRewind : MonoBehaviour
{
    bool isRewinding = false;
    bool rewindPressed = false;
    [SerializeField] float rewindIntensity = 0.02f;
    [SerializeField] RewindController rewindController;
    [SerializeField] float rewindValue = 0;

    InputSystem_Actions inputActions;

    private void OnEnable()
    {
        inputActions.Player.Rewind.started += e => TurnBackTimePressed();
        inputActions.Player.Rewind.canceled += e => TurnBackTimeReleased();

        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Player.Rewind.started -= e => TurnBackTimePressed();
        inputActions.Player.Rewind.canceled -= e => TurnBackTimeReleased();

        inputActions.Disable();
    }

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        rewindController = FindFirstObjectByType<RewindController>();
    }


    void FixedUpdate()
    {
        if (rewindPressed)
        {
            rewindValue += rewindIntensity;

            if (!isRewinding)
            {
                rewindController.StartRewindTimeBySeconds(rewindValue);
            }
            else
            {
                if (rewindController.secondsAvailableForRewind > rewindValue)
                    rewindController.SetTimeSecondsInRewind(rewindValue);
            }
            isRewinding = true;
        }
        else
        {
            if (isRewinding)
            {
                rewindController.StopRewindTimeBySeconds();
                rewindValue = 0;
                isRewinding = false;
            }
        }
    }

    public void TurnBackTimePressed()
    {
        rewindPressed = true;

        if (UIManager.instance != null)
        {
            UIManager.instance.TimeRewindStarted();
        }

    }

    public void TurnBackTimeReleased()
    {
        rewindPressed = false;

        if (UIManager.instance != null)
        {
            UIManager.instance.TimeRewindStopped();
        }
    }
}
