using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireSpellState
{
    Casting,
    Thrown
}

public class FireSpell : MonoBehaviour
{
    [Header("Projectile Behaviour")]
    public bool castByPlayer;
    public float speed = 3.0f;
    public float lifeTime = 5.0f;

    Rigidbody2D rb;
    float timeSinceCreation = 0.0f;
    FireSpellState state = FireSpellState.Casting;
    
    [Header("Animation Parameters")]
    public float castSpeed = 1.0f;
    public float idleSpeed = 1.0f;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Getting the rigidbody of the fireball
        rb = GetComponent<Rigidbody2D>();
        
        // Setting animator values and running the cast animation
        animator = GetComponent<Animator>();
        animator.SetFloat("Cast Speed", castSpeed);
        animator.SetFloat("Idle Speed", idleSpeed);
        animator.SetTrigger("Cast");
    }

    // Update is called once per frame
    void Update()
    {
        // Adding the time since the previous frame to the time since creation
        timeSinceCreation += Time.deltaTime;

        // If the time since creation has exceeded the lifetime, the spell is destroyed
        if (timeSinceCreation > lifeTime) Destroy(gameObject);
    }

    // Calls when gameObject collides with another object
    void OnCollisionEnter2D()
    {
        Explosion();
    }

    public void Throw()
    {
        state = FireSpellState.Thrown;
        rb.AddForce(transform.up * speed, ForceMode2D.Impulse);
    }

    // Handles the fireball explosion
    public void Explosion()
    {
        Destroy(gameObject);
    }
}
