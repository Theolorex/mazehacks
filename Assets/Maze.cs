using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [Header("Maze Parameters.")]
    public int w;
    public int h;

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

    private void OnDrawGizmos()
    {

    for (int x = 0; x < w; x++)
        for (int y = 0; y < h; y++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawCube(new Vector3(x, y, 0f), Vector3.one * 0.9f);
        }
    }
}