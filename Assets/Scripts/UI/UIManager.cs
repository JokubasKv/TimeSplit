using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("UI Elements")]
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image dashBarImage;
    [SerializeField] public TextMeshProUGUI promptText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverPointsTotal;

    [Header("Player Hurt Elements")]
    [SerializeField] public Image hurtImage;
    [SerializeField] public float flashDuration = 0.2f;
    [SerializeField] public float flashAlpha = 0.6f;

    [Header("Point System Elements")]
    [SerializeField] public TextMeshProUGUI pointsText;
    [SerializeField] public TextMeshProUGUI pointEntryText;

    [Header("TimeClock Elements")]
    [SerializeField] private TimeClockHandController timeClockHandController;

    [Header("PostProcessing Elements")]
    [SerializeField] private PostProcessingManager postProcessingManager;

    [SerializeField] public static bool isPaused = false;

    public void SetHealthBar(float fillAmount)
    {
        healthBarImage.fillAmount = fillAmount;
    }

    public void SetDashBar(float fillAmount)
    {
        dashBarImage.fillAmount = fillAmount;
    }

    public void SetTimeBar(float percentage)
    {
        if (timeClockHandController == null)
        {
            timeClockHandController = FindFirstObjectByType<TimeClockHandController>();
        }
        else
        {
            timeClockHandController.SetClockHandRotation(percentage);
        }
    }

    public void SetPromptText(string text)
    {
        promptText.text = text;
    }

    public void TimeRewindStarted()
    {
        postProcessingManager.StartRewind();
    }

    public void TimeRewindStopped()
    {
        postProcessingManager.StopRewind();
    }

    public void PauseResumeGame()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GotoMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadMainMenuCoroutine());
    }

    IEnumerator LoadMainMenuCoroutine()
    {
        loadingPanel.SetActive(true);
        pausePanel.SetActive(false);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TitleScene");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Destroy(gameObject);
    }

    public void SetLoadingScreen()
    {
        loadingPanel.SetActive(true);
    }

    public void OffLoadingScreen()
    {
        loadingPanel.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void TriggerHurt()
    {
        if (hurtImage != null)
            StartCoroutine(HurtFlashRoutine());
    }

    IEnumerator HurtFlashRoutine()
    {
        Color originalColor = hurtImage.color;
        Color flashColor = originalColor;
        flashColor.a = flashAlpha;
        hurtImage.color = flashColor;
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(flashAlpha, 0f, elapsed / flashDuration);
            flashColor.a = alpha;
            hurtImage.color = flashColor;
            yield return null;
        }
        flashColor.a = 0f;
        hurtImage.color = flashColor;
    }

    public void SetPointEntryText(List<string> texts, int totalPoints)
    {
        string pointEntryTextValue = string.Join("\n*", texts);
        pointEntryText.text = pointEntryTextValue;
        SetTotalPoints(totalPoints);
    }

    private void SetTotalPoints(int totalPoints)
    {
        pointsText.text = $"Points: {totalPoints.ToString()}";
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;

        gameOverPointsTotal.text = $"Total Points: {PointsController.instance.TotalPoints}";
    }

    public void Retry()
    {
        LevelManager.instance.ResetGame();
    }

    public void RemoveLoadingStuff()
    {
        loadingPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);
        SetTotalPoints(PointsController.instance.TotalPoints);
    }

    protected override void InternalInit()
    {
        if (hurtImage != null)
        {
            hurtImage.color = new Color(hurtImage.color.r, hurtImage.color.g, hurtImage.color.b, 0f);
        }
    }

    protected override void InternalOnDestroy()
    {

    }
}