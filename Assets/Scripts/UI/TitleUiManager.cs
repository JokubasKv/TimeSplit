using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUiManager : MonoBehaviour
{
    public GameObject loadingPanel;
    public GameObject mainMenuPanel;

    public void ClickOnPlay()
    {
        StartCoroutine(LoadGameLevel());
    }

    IEnumerator LoadGameLevel()
    {
        loadingPanel.SetActive(true);
        mainMenuPanel.SetActive(false);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void ClickOnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
