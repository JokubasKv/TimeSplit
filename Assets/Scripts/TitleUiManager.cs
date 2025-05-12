using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUiManager : MonoBehaviour
{
    public void ClickOnPlay()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ClickOnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
