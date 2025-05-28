using UnityEngine;

public class ExampleSceneSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public GameObject[] spawnedObjects;

    public void SpawnGrid(int count = 5000)
    {
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));
        spawnedObjects = new GameObject[count];
        float spacing = 2.0f;

        for (int i = 0; i < count; i++)
        {
            int x = i % gridSize;
            int z = i / gridSize;
            Vector3 position = new Vector3(x * spacing, 0, z * spacing);
            spawnedObjects[i] = Instantiate(prefabToSpawn, position, Quaternion.identity);
        }
    }
    public void MoveSpawnedObjectsRandomly(float forceMagnitude = 10f)
    {
        if (spawnedObjects == null) return;

        foreach (GameObject obj in spawnedObjects)
        {
            if (obj == null) continue;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDirection = Random.onUnitSphere;
                rb.AddForce(randomDirection.normalized * forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}
