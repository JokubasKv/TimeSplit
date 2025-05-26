using System.Collections;
using UnityEngine;

public class LevelEnder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!RewindController.instance.IsBeingRewinded)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(EndLevel());
            }
        }
    }

    IEnumerator EndLevel()
    {
        UIManager.instance.TimeRewindStarted();
        yield return new WaitForSeconds(1f);
        UIManager.instance.TimeRewindStopped();

        LevelManager.instance.NextLevel();
    }
}
