using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMan : MonoBehaviour

    
{
    
    public int chestCount = 0;
    GameObject maze;
    // Start is called before the first frame update
    void Start()
    {
        
        List<GameObject> chests = new List<GameObject>();

        foreach (Transform child in maze.transform)
        {
            if (child.name == "Chest")
            {
                chestCount++;
                chests.Add(child.gameObject);
            }
        }
        
        Debug.Log("Chests found: " + chestCount);

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
        
    }
}
