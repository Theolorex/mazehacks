 using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [Header("Maze Size")]
    static public GameObject entrance;
    public int w;
    public int h;

    public Sprite horizChess;
    public Sprite vertChess;
    public Sprite horizWall;
    public Sprite vertWall;
    static public int chestCount = 2;
    

    [Header("Courtyard Size")]
    public int courtyardCount = 2;
    public int courtyardSize = 2;

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private GameObject entrancePrefab;

    [Range(0f, 1f)]
    public float loopPercent = 0.1f;

    private int[,] tiles;

    public int Get(int x, int y)
    {
        return tiles[x, y];
    }

    private void Set(int v, int x, int y)
    {
        tiles[x, y] = v;
    }

    private void Start()
    {
        Generate();
    }

    private void Fill(int v)
    {
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                tiles[x, y] = v;
            }
        }
    }

    private bool InMaze(int x, int y)
    {
        return x >= 1 && y >= 1 && x <= w - 2 && y <= h - 2;
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        tiles = new int[w, h];
        Fill(1);

        for (int x = 1; x <= w - 2; x += 2)
        {
            for (int y = 1; y <= h - 2; y += 2)
            {
                Set(0, x, y);
            }
        }

        bool[,] visited = new bool[w, h];

        Walk(1, 1, visited);
        AddLoops(loopPercent);
        CreateCourtyards(courtyardCount, courtyardSize);
        PlaceChests(chestCount);
        CreateColliders();
    }

    private void Walk(int x, int y, bool[,] visited)
    {
        visited[x, y] = true;

        Vector2Int[] directions =
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        for (int i = 0; i < directions.Length; i++)
        {
            int j = Random.Range(i, directions.Length);

            Vector2Int temp = directions[i];
            directions[i] = directions[j];
            directions[j] = temp;
        }

        foreach (Vector2Int direction in directions)
        {
            int nx = x + direction.x * 2;
            int ny = y + direction.y * 2;

            if (InMaze(nx, ny) && !visited[nx, ny])
            {
                Set(
                    0,
                    x + direction.x,
                    y + direction.y
                );

                Walk(nx, ny, visited);
            }
        }
    }

    private void AddLoops(float percent)
    {
        List<int> wallsX = new List<int>();
        List<int> wallsY = new List<int>();

        for (int x = 1; x <= w - 2; x += 2)
        {
            for (int y = 1; y <= h - 2; y += 2)
            {
                if (InMaze(x + 2, y))
                {
                    wallsX.Add(x + 1);
                    wallsY.Add(y);
                }

                if (InMaze(x, y + 2))
                {
                    wallsX.Add(x);
                    wallsY.Add(y + 1);
                }
            }
        }

        int n = wallsX.Count;
        int count = Mathf.FloorToInt(n * percent);

        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(i, n);

            (wallsX[i], wallsX[j]) = (wallsX[j], wallsX[i]);
            (wallsY[i], wallsY[j]) = (wallsY[j], wallsY[i]);

            Set(0, wallsX[i], wallsY[i]);
        }
    }

    private void CreateCourtyards(int count, int size)
    {

        int maxStartX = (w - 2) - (size - 1);
        int maxStartY = (h - 2) - (size - 1);

        for (int i = 0; i < count; i++)
        {
            int startX = Random.Range(1, maxStartX + 1);
            int startY = Random.Range(1, maxStartY + 1);

            for (int x = startX; x < startX + size; x++)
            {
                for (int y = startY; y < startY + size; y++)
                {
                    Set(0, x, y);
                }
            }
        }
    }
    private List<Vector2Int> FindDeadEnds()
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        for (int x = 1; x <= w - 2; x += 2)
        {
            for (int y = 1; y <= h - 2; y += 2)
            {
                if (tiles[x, y] != 0) continue;

                int neighbors = 0;

                if (x + 1 < w && tiles[x + 1, y] == 0) neighbors++;
                if (x - 1 >= 0 && tiles[x - 1, y] == 0) neighbors++;

                if (y + 1 < h && tiles[x, y + 1] == 0) neighbors++;
                if (y - 1 >= 0 && tiles[x, y - 1] == 0) neighbors++;

                if (neighbors == 1)
                {
                    deadEnds.Add(new Vector2Int(x, y));
                }
            }
        }

        return deadEnds;
    }

    private void PlaceChests(int count)
    {
        List<Vector2Int> deadEnds = FindDeadEnds();

        for (int i = 0; i < deadEnds.Count; i++)
        {
            int j = Random.Range(i, deadEnds.Count);
            (deadEnds[i], deadEnds[j]) = (deadEnds[j], deadEnds[i]);
        }

        int placed = 0;
        for (int i = 0; i < deadEnds.Count && placed < count; i++)
        {
            Set(2, deadEnds[i].x, deadEnds[i].y);

            placed++;
        }
    }


    private void CreateColliders()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        Vector2Int randomEdge = GetRandomEdge();
        if (randomEdge != new Vector2Int(-1, -1))
        {
            Set(0, randomEdge.x, randomEdge.y); // open the wall so the grid loop below builds floor there
        }

        for (int tx = 0; tx < w; tx++)
        {
            for (int ty = 0; ty < h; ty++)
            {
                if (tiles[tx, ty] == 1)
                {
                    

                    int wallDirection = GetWallDirection(tx, ty);
                    GameObject wall = Instantiate(wallPrefab, new Vector3(tx, ty, 0), Quaternion.identity, transform);
                    if (wallDirection == 1)
                    {
                        wall.GetComponent<SpriteRenderer>().sprite = horizWall;
                    }
                    else if (wallDirection == 2)
                    {
                        wall.GetComponent<SpriteRenderer>().sprite = vertWall;
                    }
                    wall.name = "WallGood";
                }
                else if (tiles[tx, ty] == 0)
                {
                    GameObject floor = Instantiate(floorPrefab, new Vector3(tx, ty, 0), Quaternion.identity, transform);
                    floor.name = "Floor";
                }
                else if (tiles[tx, ty] == 2)
                {
                    GameObject floor = Instantiate(floorPrefab, new Vector3(tx, ty, 0), Quaternion.identity, transform);
                    floor.name = "Floor";
                    GameObject chest = Instantiate(chestPrefab, new Vector3(tx, ty, 0), Quaternion.identity, transform);
                    int direction = CheckDirection(tx, ty);
                    if (direction == 1)
                    {
                        Debug.Log("Chest at " + tx + ", " + ty + " is horizontal.");
                        chest.GetComponent<SpriteRenderer>().sprite = horizChess;
                        chest.layer = LayerMask.NameToLayer("Horiz");

                    }
                    else if (direction == 2)
                    {
                        Debug.Log("Chest at " + tx + ", " + ty + " is horizontal.");
                        chest.GetComponent<SpriteRenderer>().sprite = horizChess;
                        chest.transform.rotation = Quaternion.Euler(0, 180, 0);
                        chest.layer = LayerMask.NameToLayer("Horiz");
                    }
                    else if (direction == 3)
                    {
                        Debug.Log("Chest at " + tx + ", " + ty + " is vertical.");
                        chest.GetComponent<SpriteRenderer>().sprite = vertChess;
                        chest.layer = LayerMask.NameToLayer("Vert");
                    }
                    else if (direction == 4)
                    {
                        Debug.Log("Chest at " + tx + ", " + ty + " is vertical.");
                        chest.GetComponent<SpriteRenderer>().sprite = vertChess;
                        chest.transform.rotation = Quaternion.Euler(180, 0, 0);
                        chest.layer = LayerMask.NameToLayer("Vert");
                    }   
                    else
                    {
                        Debug.LogWarning("No valid direction found for chest at " + tx + ", " + ty);
                    }
                    chest.name = "Chest";
                }

            }

        }

        if (randomEdge != new Vector2Int(-1, -1))
        {
            entrance = Instantiate(entrancePrefab, new Vector3(randomEdge.x, randomEdge.y, 0), Quaternion.identity, transform);
            entrance.name = "Entrance";
        }
    }
    private void OnDrawGizmos()
    {
        if (tiles == null)
            return;

        for (int tx = 0; tx < w; tx++)
        {
            for (int ty = 0; ty < h; ty++)
            {
                if (tiles[tx, ty] == 2)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(
                    new Vector3(tx, ty, 0f),
                    Vector3.one * 0.9f
                    );
                }
            }
        }
    }

    private int CheckDirection(int x, int y)
    {
        //check which direction the floor that leads into the chest is located
        if (x > 0 && tiles[x - 1, y] == 0)
        {
            return 1; // left
        }
        if (x < w - 1 && tiles[x + 1, y] == 0)
        {
            return 2; // right
        }
        if (y > 0 && tiles[x, y - 1] == 0)
        {
            return 3; // down
        }
        if (y < h - 1 && tiles[x, y + 1] == 0)
        {
            return 4; // up
        }
        return 0; // no direction found
    }

    private int GetWallDirection(int x, int y)
    {
        // horizWall has a baked-in bottom shadow, so it only looks right when there's open ground below to cast it on
        bool floorBelow = y > 0 && (tiles[x, y - 1] == 0 || tiles[x, y - 1] == 2);
        return floorBelow ? 1 : 2; // 1 = horizWall (shadow), 2 = vertWall (no shadow)
    }


    private Vector2Int GetRandomEdge()
    {
        List<Vector2Int> edges = new List<Vector2Int>();

        for (int x = 1; x < w - 1; x++) // skip corners, they're covered by the y-loop below
        {
            if (tiles[x, 1] == 0) edges.Add(new Vector2Int(x, 0));
            if (tiles[x, h - 2] == 0) edges.Add(new Vector2Int(x, h - 1));
        }
        for (int y = 1; y < h - 1; y++)
        {
            if (tiles[1, y] == 0) edges.Add(new Vector2Int(0, y));
            if (tiles[w - 2, y] == 0) edges.Add(new Vector2Int(w - 1, y));
        }

        if (edges.Count == 0)
            return new Vector2Int(-1, -1);

        Vector2Int randomEdge = edges[Random.Range(0, edges.Count)];
        return randomEdge;
    }
}




#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(Maze))]
public class MazeInspector : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
 
        Maze maze = (Maze)target;
 
        GUILayout.Space(10);
 
        if (GUILayout.Button("Generate Maze"))
        {
            maze.Generate();
            UnityEditor.EditorUtility.SetDirty(maze);
        }
    }
}
#endif