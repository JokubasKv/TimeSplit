using UnityEngine;
using UnityEngine.InputSystem;

public class PressToRewind : MonoBehaviour
{
    bool isRewinding = false;
    bool rewindPressed = false;
    [SerializeField] float rewindIntensity = 0.02f;          //Variable to change rewind speed
    [SerializeField] RewindManager rewindManager;
    [SerializeField] float rewindValue = 0;

    InputSystem_Actions inputActions;

    #region -Awake/OnEnable/OnDisable -
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
        rewindManager = FindFirstObjectByType<RewindManager>();
    }
    #endregion


    void FixedUpdate()
    {
        if (rewindPressed)                     //Change keycode for your own custom key if you want
        {
            rewindValue += rewindIntensity;                 //While holding the button, we will gradually rewind more and more time into the past

            if (!isRewinding)
            {
                rewindManager.StartRewindTimeBySeconds(rewindValue);
            }
            else
            {
                if (rewindManager.secondsAvailableForRewind > rewindValue)      //Safety check so it is not grabbing values out of the bounds
                    rewindManager.SetTimeSecondsInRewind(rewindValue);
            }
            isRewinding = true;
        }
        else
        {
            if (isRewinding)
            {
                rewindManager.StopRewindTimeBySeconds();
                //rewindSound.Stop();
                rewindValue = 0;
                isRewinding = false;
            }
        }
    }

    void TurnBackTimePressed()
    {
        rewindPressed = true;
    }

    void TurnBackTimeReleased()
    {
        rewindPressed = false;
    }
}
