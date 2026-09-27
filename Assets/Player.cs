using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public Vector2 direction;
    private Rigidbody2D rigidbody2D;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

        public Vector2 GetPosition()
    {
        return transform.position;
    }
    public Vector2 GetDirection()
    {
        return direction;
    }



    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        SpawnOnRandomFloor();
    }

    private void SpawnOnRandomFloor()
    {
        Maze maze = FindObjectOfType<Maze>();
        if (maze == null)
        {
            Debug.LogWarning("Player.SpawnOnRandomFloor: no Maze found in scene.");
            return;
        }

        List<Vector2Int> floorTiles = new List<Vector2Int>();
        for (int x = 0; x < maze.w; x++)
        {
            for (int y = 0; y < maze.h; y++)
            {
                if (maze.Get(x, y) == 0)
                {
                    floorTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        if (floorTiles.Count == 0)
        {
            // empty means Maze.Start() likely hasn't generated tiles yet (Start() order isn't guaranteed across scripts)
            Debug.LogWarning("Player.SpawnOnRandomFloor: maze has no floor tiles yet.");
            return;
        }

        Vector2Int spawn = floorTiles[Random.Range(0, floorTiles.Count)];
        transform.position = new Vector3(spawn.x, spawn.y, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }
}
