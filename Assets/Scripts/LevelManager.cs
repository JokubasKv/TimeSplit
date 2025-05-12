using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoSingleton<LevelManager>
{
    protected override void InternalInit()
    {
    }

    protected override void InternalOnDestroy()
    {
    }

    public void ResetGame()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        Destroy(player);
        SceneManager.LoadScene("GameScene");
    }
}
