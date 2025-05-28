using System.Collections;
using System.Linq;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RewindPositionPerformanceTest
{
    private const int ObjectCount = 5000;
    private const string PrefabPath = "Prefabs/RewindCube";
    private GameObject[] spawnedObjects;
    private RewindController rewindManager;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        SceneManager.LoadScene("RewindTestingScene", LoadSceneMode.Single);

        yield return null;

        // Load prefab
        var prefab = Resources.Load<GameObject>(PrefabPath);
        Assert.IsNotNull(prefab, $"Prefab not found at Resources/{PrefabPath}");

        // Spawn objects
        spawnedObjects = new GameObject[ObjectCount];
        for (int i = 0; i < ObjectCount; i++)
        {
            spawnedObjects[i] = GameObject.Instantiate(prefab, Random.insideUnitSphere * 10f, Quaternion.identity);
        }

        // Find or create RewindManager
        rewindManager = GameObject.FindFirstObjectByType<RewindController>();
        if (rewindManager == null)
        {
            var managerGO = new GameObject("RewindManager");
            rewindManager = managerGO.AddComponent<RewindController>();
        }

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (spawnedObjects != null)
        {
            foreach (var obj in spawnedObjects)
            {
                if (obj != null)
                    GameObject.Destroy(obj);
            }
        }
        var manager = GameObject.FindFirstObjectByType<RewindController>();
        if (manager != null)
            GameObject.Destroy(manager.gameObject);
        yield return null;
    }

    [UnityTest, Performance]
    public IEnumerator Rewind_ManyObjects_Performance()
    {
        // Simulate some time passing and objects moving
        for (int frame = 0; frame < 120; frame++)
        {
            foreach (var obj in spawnedObjects)
            {
                if (obj != null)
                    obj.transform.position += Random.onUnitSphere * 0.1f;
            }
            yield return null;
        }

        // Measure performance and memory of rewinding
        Measure.Method(() =>
        {
            rewindManager.StartRewindTimeBySeconds(rewindManager.secondsAvailableForRewind);
        })
        .WarmupCount(3)
        .MeasurementCount(10)
        .GC()
        .Run();

        yield return null;
    }

    [UnityTest, Performance]
    public IEnumerator Rewind_Gradually_Performance()
    {
        // Simulate some time passing and objects moving
        for (int frame = 0; frame < 120; frame++)
        {
            foreach (var obj in spawnedObjects)
            {
                if (obj != null)
                    obj.transform.position += Random.onUnitSphere * 0.1f;
            }
            yield return null;
        }



        var rewinder = new GameObject("PressToRewind");
        var pressToRewind = rewinder.AddComponent<PressToRewind>();


        using (Measure.Frames().Scope())
        {
            pressToRewind.TurnBackTimePressed();
            yield return new WaitForSeconds(rewindManager.secondsAvailableForRewind);
            pressToRewind.TurnBackTimeReleased();
        }

        yield return null;
    }

    [UnityTest, Performance]
    public IEnumerator Rewind_GraduallyNotMoving_Performance()
    {
        for (int frame = 0; frame < 120; frame++)
        {
            yield return null;
        }

        var rewinder = new GameObject("PressToRewind");
        var pressToRewind = rewinder.AddComponent<PressToRewind>();

        using (Measure.Frames().Scope())
        {
            pressToRewind.TurnBackTimePressed();
            yield return new WaitForSeconds(rewindManager.secondsAvailableForRewind);
            pressToRewind.TurnBackTimeReleased();
        }

        yield return null;
    }
    [UnityTest, Performance]
    public IEnumerator Rewind_Performance_NoTracking()
    {
        foreach (var obj in spawnedObjects)
        {
            obj.GetComponent<RewindAbstract>().IsTracking = false;
        }

        using (Measure.Frames().Scope())
        {
            for (int frame = 0; frame < RewindController.secondsToTrack / Time.fixedDeltaTime; frame++)
            {
                foreach (var obj in spawnedObjects)
                {
                    if (obj != null)
                        obj.transform.position += Random.onUnitSphere * 0.1f;
                }
                yield return new WaitForFixedUpdate();
            }
        }

        yield return null;
    }

    [UnityTest, Performance]
    public IEnumerator Rewind_Performance_Tracking()
    {
        using (Measure.Frames().Scope())
        {
            for (int frame = 0; frame < RewindController.secondsToTrack / Time.fixedDeltaTime; frame++)
            {
                foreach (var obj in spawnedObjects)
                {
                    if (obj != null)
                        obj.transform.position += Random.onUnitSphere * 0.1f;
                }
                yield return new WaitForFixedUpdate();
            }
        }

        yield return null;
    }

    [UnityTest, Performance]
    public IEnumerator Rewind_MemoryUsage_Tracking()
    {
        var rewindAbstracts = spawnedObjects
            .Select(obj => obj.GetComponent<RewindPosition>())
            .Where(rewind => rewind != null)
            .ToArray();

        foreach (var rewind in rewindAbstracts)
        {
            rewind.MainInit();
        }

        Measure.Method(() =>
        {
            for (int frame = 0; frame < RewindController.secondsToTrack / Time.fixedDeltaTime; frame++)
            {
                foreach (var rewind in rewindAbstracts)
                {
                    rewind.Track();
                }
            }
        })
        .WarmupCount(3)
        .MeasurementCount(10)
        .GC()
        .Run();

        yield return null;
    }

    [UnityTest, Performance]
    public IEnumerator Rewind_MemoryUsage_NoTracking()
    {
        var rewindAbstracts = spawnedObjects
            .Select(obj => obj.GetComponent<RewindAbstract>())
            .Where(rewind => rewind != null)
            .ToArray();

        foreach (var rewind in rewindAbstracts)
        {
            rewind.MainInit();
            rewind.IsTracking = false;
        }

        Measure.Method(() =>
        {
            for (int frame = 0; frame < RewindController.secondsToTrack / Time.fixedDeltaTime; frame++)
            {
                foreach (var rewind in rewindAbstracts)
                {
                    rewind.Track();
                }
            }
        })
        .WarmupCount(3)
        .MeasurementCount(10)
        .GC()
        .Run();

        yield return null;
    }
}
