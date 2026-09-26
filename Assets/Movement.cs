using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    public Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        

        
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        
        rb.velocity = movement * moveSpeed;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }
    public Vector2 GetDirection()
    {
        return direction;
    }


}
