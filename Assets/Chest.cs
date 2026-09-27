using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private ChestMan chestMan;
    // Start is called before the first frame update
    void Start()
    {
        chestMan = FindObjectOfType<ChestMan>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered chest trigger.");
        if (other.gameObject.CompareTag("Player"))
        {
            chestMan.decrementChestCount();
            this.gameObject.tag = "Untagged";
            Debug.Log("Chest collected. Remaining chests: " + chestMan.chestCount);
        }
    }
}
