using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMan : MonoBehaviour

    
{
    
    public int chestCount = 0;
    public GameObject maze;
    public List<GameObject> chests = new List<GameObject>();
    
    private bool chestsFound = false;
    public bool allChestsCollected = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public int decrementChestCount()
    {
        if (chestCount > 0)
        {
            chestCount--;
        }
        return chestCount;
    }

    // Update is called once per frame
    void Update()
    {
        // Maze.Start() may run after this script's Start(), so wait until Update to search for chests.
        if (!chestsFound)
        {
            foreach (Transform child in maze.transform)
            {
                if (child.name == "Chest")
                {
                    chestCount++;
                    chests.Add(child.gameObject);
                }
            }

            Debug.Log("Chests found: " + chestCount);
            chestsFound = true;
        }

        if (chestCount == 0 && chestsFound && !allChestsCollected)
        {
            
            Maze.entrance.tag = "OpenDoor";
            Maze.entrance.GetComponent<Collider2D>().isTrigger = true; // unlock: stop blocking and let EndGame's trigger fire
            allChestsCollected = true;
        }
    }
}
