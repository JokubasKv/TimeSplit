using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using RangeAttribute = UnityEngine.RangeAttribute;

public class DungeonMaker : MonoBehaviour
{
    [SerializeField] public int dungeonWidth;
    [SerializeField] public int dungeonHeight;

    [SerializeField] public int minRoomWidth;
    [SerializeField] public int minRoomLenght;

    [SerializeField] public int maxIterations;
    [SerializeField] public int corridorWidth;

    [SerializeField] public Material floorMaterial;
    [SerializeField] public Material roofMaterial;

    [SerializeField]
    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;
    [SerializeField]
    [Range(0.7f, 1.0f)]
    public float roomTopCornerMidifier;
    [SerializeField]
    [Range(0, 2)]
    public int roomOffset;

    [SerializeField]
    public GameObject wallVertical, wallHorizontal;
    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;


    public List<GameObject> roomObjectPrefabs;
    public List<GameObject> roomWallObjectPrefabs;
    [RangeAttribute(0.0f, 1.0f)]
    public float roomWallObjectDensity = 0.1f;
    public List<GameObject> lightPrefabs;
    public List<GameObject> enemyPrefabs;

    public GameObject startingRoomPrefab;
    public GameObject exitRoomPrefab;

    public GameObject playerPrefab;

    private List<GameObject> _dungeonFloors;

    void Start()
    {
        CreateDungeon();
    }

    public void CreateDungeon()
    {
        DestroyAllChildren();

        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonHeight);
        var listOfRooms = generator.CalculateRooms(maxIterations, minRoomWidth, minRoomLenght, roomBottomCornerModifier, roomTopCornerMidifier, roomOffset, corridorWidth);

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();

        _dungeonFloors = new List<GameObject>();

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftCorner, listOfRooms[i].TopRightCorner);
        }
        CreateWalls(wallParent);

        PopulateRoom(listOfRooms);

        foreach (var floor in _dungeonFloors.Where(d => d != null))
        {
            NavMeshSurface navMeshSurface = floor.GetComponent<NavMeshSurface>();
            navMeshSurface.BuildNavMesh();
        }

        PopulateWithEnemies(listOfRooms);
    }

    private void PopulateWithEnemies(List<AbstractNode> listOfRooms)
    {
        foreach (RoomNode room in listOfRooms.Where(r => r is RoomNode && (r as RoomNode).Type == RoomType.NormalRoom))
        {
            Vector2Int bottomLeft = room.BottomLeftCorner;
            Vector2Int topRight = room.TopRightCorner;

            // Determine the number of enemies to spawn based on room size  
            Vector2Int roomSize = new Vector2Int(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
            int enemyCount = Mathf.Max(1, Mathf.Clamp(roomSize.x * roomSize.y / 100, 0, 6)); // Adjust density as needed  

            for (int i = 0; i < enemyCount; i++)
            {
                // Randomly position the enemy within the room  
                Vector3 randomPosition = new Vector3(
                    UnityEngine.Random.Range(bottomLeft.x + 1, topRight.x - 1),
                    0,
                    UnityEngine.Random.Range(bottomLeft.y + 1, topRight.y - 1)
                );

                // Randomly select an enemy prefab  
                GameObject enemyPrefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];
                GameObject enemyInstance = Instantiate(enemyPrefab, randomPosition + enemyPrefab.transform.position, Quaternion.identity, transform);

                // Check if the enemy is a patrolling enemy  
                PatrollingEnemy patrollingEnemy = enemyInstance.GetComponent<PatrollingEnemy>();
                if (patrollingEnemy != null)
                {
                    // Attach EnemyPath component if not already present  
                    EnemyPath enemyPathComponent = enemyInstance.GetComponent<EnemyPath>();
                    if (enemyPathComponent == null)
                    {
                        enemyPathComponent = enemyInstance.AddComponent<EnemyPath>();
                    }

                    // Generate a patrol path for the patrolling enemy  
                    List<Vector3> pathPoints = GenerateEnemyPathPoints(bottomLeft, topRight);

                    // Create empty GameObjects for each path point and assign them to the path  
                    List<GameObject> pathPointObjects = new List<GameObject>();
                    foreach (var point in pathPoints)
                    {
                        GameObject pathPointObject = new GameObject("PathPoint");
                        pathPointObject.transform.position = point;
                        pathPointObject.transform.parent = transform;
                        pathPointObjects.Add(pathPointObject);
                    }

                    enemyPathComponent.waypoints = pathPointObjects.Select(p => p.transform).ToList();
                    patrollingEnemy.path = enemyPathComponent;
                }
            }
        }
    }

    private List<Vector3> GenerateEnemyPathPoints(Vector2Int bottomLeft, Vector2Int topRight)
    {
        List<Vector3> patrolPath = new List<Vector3>();
        int pathPoints = UnityEngine.Random.Range(3, 6); // Random number of patrol points

        for (int i = 0; i < pathPoints; i++)
        {
            Vector3 randomPoint = new Vector3(
                UnityEngine.Random.Range(bottomLeft.x + 1, topRight.x - 1),
                0,
                UnityEngine.Random.Range(bottomLeft.y + 1, topRight.y - 1)
            );
            patrolPath.Add(randomPoint);
        }

        return patrolPath;
    }

    private void PopulateRoom(List<AbstractNode> listOfRooms)
    {
        foreach (RoomNode node in listOfRooms.Where(n => n is RoomNode))
        {
            Vector2Int bottomLeft = node.BottomLeftCorner;
            Vector2Int topRight = node.TopRightCorner;
            Vector2Int roomSize = new Vector2Int(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);

            switch (node.Type)
            {
                case RoomType.StartingRoom:
                    CreateInRoomMiddle(startingRoomPrefab, node);
                    CreateInRoomMiddlePlayer(playerPrefab, node);
                    break;
                case RoomType.ExitRoom:
                    CreateInRoomMiddle(exitRoomPrefab, node);
                    break;
                case RoomType.NormalRoom:
                    CreateRoomObjectsRandom(bottomLeft, topRight, roomSize);
                    PlaceObjectNearWall(bottomLeft, topRight);
                    break;
            }

            CreateGridLights(bottomLeft, topRight);
        }
    }

    private void CreateRoomObjectsRandom(Vector2Int bottomLeft, Vector2Int topRight, Vector2Int roomSize)
    {
        int objectCount = Mathf.Max(1, roomSize.x * roomSize.y / 10); // Adjust density as needed
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = new Vector3(
                UnityEngine.Random.Range(bottomLeft.x + 1, topRight.x - 1),
                0,
                UnityEngine.Random.Range(bottomLeft.y + 1, topRight.y - 1)
            );

            Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90, 0);

            GameObject randomPrefab = roomObjectPrefabs[UnityEngine.Random.Range(0, roomObjectPrefabs.Count)];
            Instantiate(randomPrefab, randomPosition + randomPrefab.transform.position, randomRotation, transform);
        }
    }

    private void CreateGridLights(Vector2Int bottomLeft, Vector2Int topRight)
    {
        float spacing = 6.0f;
        for (float x = bottomLeft.x + spacing / 2; x < topRight.x; x += spacing)
        {
            for (float z = bottomLeft.y + spacing / 2; z < topRight.y; z += spacing)
            {
                Vector3 lightPosition = new Vector3(x, 5f, z);
                GameObject lightPrefab = lightPrefabs[UnityEngine.Random.Range(0, lightPrefabs.Count)];
                Instantiate(lightPrefab, lightPosition + lightPrefab.transform.position, Quaternion.identity, transform);
            }
        }
    }

    private void CreateInRoomMiddle(GameObject prefab, RoomNode node)
    {
        Vector2Int bottomLeft = node.BottomLeftCorner;
        Vector2Int topRight = node.TopRightCorner;
        Vector3 roomMiddle = new Vector3(
            (bottomLeft.x + topRight.x) / 2.0f,
            0,
            (bottomLeft.y + topRight.y) / 2.0f
        );

        Instantiate(prefab, roomMiddle + prefab.transform.position, Quaternion.identity, transform);
    }

    private void CreateInRoomMiddlePlayer(GameObject prefab, RoomNode node)
    {
        Vector2Int bottomLeft = node.BottomLeftCorner;
        Vector2Int topRight = node.TopRightCorner;
        Vector3 roomMiddle = new Vector3(
            (bottomLeft.x + topRight.x) / 2.0f,
            0,
            (bottomLeft.y + topRight.y) / 2.0f
        );

        Instantiate(prefab, roomMiddle + prefab.transform.position, Quaternion.identity);
    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            CreateWall(wallParent, wallPosition, wallHorizontal);
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            CreateWall(wallParent, wallPosition, wallVertical);
        }
    }

    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        Instantiate(
            wallPrefab,
            (Vector3)wallPosition + wallPrefab.transform.position,
            wallPrefab.transform.rotation,
            wallParent.transform);
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
           topLeftV,
           topRightV,
           bottomLeftV,
           bottomRightV
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
           0,
           1,
           2,
           2,
           1,
           3
        };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(NavMeshSurface));
        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = floorMaterial;
        dungeonFloor.GetComponent<MeshCollider>().sharedMesh = mesh;
        dungeonFloor.transform.parent = transform;

        _dungeonFloors.Add(dungeonFloor);

        int[] triangles2 = new int[]
        {
           0,
           2,
           1,
           1,
           2,
           3
        };

        Mesh mesh2 = new Mesh();
        mesh2.vertices = vertices;
        mesh2.uv = uvs;
        mesh2.triangles = triangles2;

        GameObject dungeonRoof = new GameObject("Roof" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        dungeonRoof.transform.position = Vector3.zero + new Vector3(0, 5f, 0);
        dungeonRoof.transform.localScale = Vector3.one;
        dungeonRoof.GetComponent<MeshFilter>().mesh = mesh2;
        dungeonRoof.GetComponent<MeshRenderer>().material = roofMaterial;
        dungeonRoof.GetComponent<MeshCollider>().sharedMesh = mesh2;
        dungeonRoof.transform.parent = transform;

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    private void DestroyAllChildren()
    {
        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
    private void PlaceObjectNearWall(Vector2Int bottomLeft, Vector2Int topRight)
    {
        // Iterate over the horizontal walls  
        for (int x = bottomLeft.x + 1; x < topRight.x; x++) // Adjusted to stay inside the walls
        {
            if (UnityEngine.Random.value < roomWallObjectDensity) // Randomly decide whether to place an object  
            {
                // Bottom wall  
                Vector3 positionBottom = new Vector3(x, 0, bottomLeft.y + 0.5f); // Adjusted position to be inside
                Quaternion rotationBottom = Quaternion.Euler(0, 180, 0);
                InstantiateObjectNearWall(positionBottom, rotationBottom);
            }

            if (UnityEngine.Random.value < roomWallObjectDensity) // Randomly decide whether to place an object  
            {
                // Top wall  
                Vector3 positionTop = new Vector3(x, 0, topRight.y - 0.5f); // Adjusted position to be inside
                Quaternion rotationTop = Quaternion.Euler(0, 180, 0);
                InstantiateObjectNearWall(positionTop, rotationTop);
            }
        }

        // Iterate over the vertical walls  
        for (int z = bottomLeft.y + 1; z < topRight.y; z++) // Adjusted to stay inside the walls
        {
            if (UnityEngine.Random.value < roomWallObjectDensity) // Randomly decide whether to place an object  
            {
                // Left wall  
                Vector3 positionLeft = new Vector3(bottomLeft.x + 0.5f, 0, z); // Adjusted position to be inside
                Quaternion rotationLeft = Quaternion.Euler(0, -90, 0);
                InstantiateObjectNearWall(positionLeft, rotationLeft);
            }

            if (UnityEngine.Random.value < roomWallObjectDensity) // Randomly decide whether to place an object  
            {
                // Right wall  
                Vector3 positionRight = new Vector3(topRight.x - 0.5f, 0, z); // Adjusted position to be inside
                Quaternion rotationRight = Quaternion.Euler(0, 90, 0);
                InstantiateObjectNearWall(positionRight, rotationRight);
            }
        }
    }

    private void InstantiateObjectNearWall(Vector3 position, Quaternion rotation)
    {
        if (roomWallObjectPrefabs.Count == 0) return;

        GameObject prefab = roomWallObjectPrefabs[UnityEngine.Random.Range(0, roomWallObjectPrefabs.Count)];
        Instantiate(prefab, position, rotation, transform);
    }
}
