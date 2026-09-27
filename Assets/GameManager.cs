using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RegenerateMap();
        }
    }

    private void RegenerateMap()
    {
        Time.timeScale = 1; // in case a fail state paused it

        Maze maze = FindObjectOfType<Maze>();
        if (maze == null)
        {
            Debug.LogWarning("GameManager.RegenerateMap: no Maze found in scene.");
            return;
        }
        maze.Generate();

        if (Player.Instance != null)
        {
            Player.Instance.SpawnOnRandomFloor();
        }

        MinotaurManager minotaur = FindObjectOfType<MinotaurManager>();
        if (minotaur != null)
        {
            minotaur.Respawn();
        }
    }



    public void GameFail()
    {
        Time.timeScale = 0;
        
        
    }
    
    
}
