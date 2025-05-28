using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoSingleton<LevelManager>
{
    public bool remakePlayer = true;

    public int level = 0;

    protected override void InternalInit()
    {
    }

    protected override void InternalOnDestroy()
    {
    }

    public void NextLevel()
    {
        remakePlayer = false;
        level++;
        StartCoroutine(LoadGameSceneCouroutine());
    }

    public void ResetGame()
    {
        remakePlayer = true;
        level = 0;
        //PointsController.instance.ResetPoints();
        StartCoroutine(LoadGameSceneCouroutine());
    }

    IEnumerator LoadGameSceneCouroutine()
    {
        UIManager.instance.SetLoadingScreen();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        UIManager.instance.RemoveLoadingStuff();
    }
}
