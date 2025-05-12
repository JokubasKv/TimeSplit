using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("UI Elements")]
    [SerializeField] private Image healthbarImage;
    [SerializeField] public TextMeshProUGUI promptText;

    [Header("TimeClock Elements")]
    [SerializeField] private TimeClockHandController timeClockHandController;

    [Header("PostProcessing Elements")]
    [SerializeField] private PostProcessingManager postProcessingManager;

    [SerializeField] public bool paused = false;

    public void SetHealthBar(float fillAmount)
    {
        healthbarImage.fillAmount = fillAmount;
    }

    public void SetTimeBar(float percentage)
    {
        if (timeClockHandController == null)
        {
            timeClockHandController = FindFirstObjectByType<TimeClockHandController>();
        }
        timeClockHandController.SetClockHandRotation(percentage);
    }

    public void TimeRewindStarted()
    {
        postProcessingManager.StartRewind();
    }

    public void TimeRewindStopped()
    {
        postProcessingManager.StopRewind();
    }

    protected override void InternalInit()
    {
    }

    protected override void InternalOnDestroy()
    {

    }
}