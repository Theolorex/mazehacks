using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    public Vector2 direction;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite sideSprite;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        

        
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        if (movement.x > 0)
        {
            GetComponent<SpriteRenderer>().sprite = sideSprite;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (movement.x < 0)
        {
            GetComponent<SpriteRenderer>().sprite = sideSprite;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (movement.y > 0)
        {
            GetComponent<SpriteRenderer>().sprite = upSprite;
        }
        else if (movement.y < 0)
        {
            GetComponent<SpriteRenderer>().sprite = downSprite;
        }
        rb.velocity = movement * moveSpeed;
    }




}
