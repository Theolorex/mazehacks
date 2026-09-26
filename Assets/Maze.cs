 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [Header("Maze Size")]
    public int w;
    public int h;

    public int chestCount = 3;

    [Header("Courtyard Size")]
    public int courtyardCount = 2;
    public int courtyardSize = 2;

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject floorPrefab;

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
        for (int tx = 0; tx < w; tx++)
        {
            for (int ty = 0; ty < h; ty++)
            {
                if (tiles[tx, ty] == 1)
                {
                    GameObject wall = Instantiate(wallPrefab, new Vector3(tx, ty, 0), Quaternion.identity, transform);
                    wall.name = "WallGood";
                }
                else if (tiles[tx, ty] == 0)
                {
                    GameObject chest = Instantiate(floorPrefab, new Vector3(tx, ty, 0), Quaternion.identity, transform);
                    chest.name = "Floor";
                }
            }

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