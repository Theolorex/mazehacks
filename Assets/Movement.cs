using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxMoveSpeed = 10f;
    public float acceleration = 2f; // speed gained per second movement is held
    private float holdTime = 0f;
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

        holdTime = movement.sqrMagnitude > 0f ? holdTime + Time.fixedDeltaTime : 0f;
        float currentSpeed = Mathf.Min(moveSpeed + acceleration * holdTime, maxMoveSpeed);

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
        rb.velocity = movement * currentSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            holdTime = 0f;
        }
    }

}

