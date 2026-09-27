using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private ChestMan chestMan;
    public Sprite openedChestVert;
    public Sprite openedChestHoriz;
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
        if (other.gameObject.CompareTag("Player") )
        {
            chestMan.decrementChestCount();
            this.gameObject.tag = "OpenedChest";
            if (this.gameObject.layer == LayerMask.NameToLayer("Vert"))
            {
                this.GetComponent<SpriteRenderer>().sprite = openedChestVert;
            }
            else if (this.gameObject.layer == LayerMask.NameToLayer("Horiz"))
            {
                this.GetComponent<SpriteRenderer>().sprite = openedChestHoriz;
            }
            Debug.Log("Chest collected. Remaining chests: " + chestMan.chestCount);
        }
    }
}
