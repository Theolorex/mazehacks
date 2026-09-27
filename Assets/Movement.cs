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
    public ParticleSystem accelerationParticles;
    public float maxParticleRate = 50f; // emission rate once fully accelerated to maxMoveSpeed
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 10f;
    private float dashCooldownTimer = 0f;
    private bool isDashing = false;
    private Vector2 lastMoveDirection = Vector2.down;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (accelerationParticles != null)
        {
            accelerationParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = 2; // draw above walls/chests (order 1) and floor (order 0)
        }
    }

    private void Update()
    {
        dashCooldownTimer = Mathf.Max(0f, dashCooldownTimer - Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && !isDashing)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        if (movement.sqrMagnitude > 0f)
        {
            lastMoveDirection = movement;
        }

        holdTime = movement.sqrMagnitude > 0f ? holdTime + Time.fixedDeltaTime : 0f;
        float currentSpeed = Mathf.Min(moveSpeed + acceleration * holdTime, maxMoveSpeed);
        UpdateAccelerationParticles(currentSpeed);

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

        if (!isDashing)
        {
            rb.velocity = movement * currentSpeed;
        }
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        Vector2 dashDirection = lastMoveDirection.normalized;
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            rb.velocity = dashDirection * dashSpeed;
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            holdTime = 0f;
            isDashing = false; // stop dashing into the wall so normal movement resumes
        }
    }

    private void UpdateAccelerationParticles(float currentSpeed)
    {
        if (accelerationParticles == null) return;

        bool isAccelerating = currentSpeed > moveSpeed;
        if (isAccelerating && !accelerationParticles.isPlaying)
        {
            accelerationParticles.Play();
        }
        else if (!isAccelerating && accelerationParticles.isPlaying)
        {
            accelerationParticles.Stop();
        }

        float accelerationProgress = Mathf.InverseLerp(moveSpeed, maxMoveSpeed, currentSpeed);
        ParticleSystem.EmissionModule emission = accelerationParticles.emission;
        emission.rateOverTime = Mathf.Lerp(0f, maxParticleRate, accelerationProgress);
    }

}

