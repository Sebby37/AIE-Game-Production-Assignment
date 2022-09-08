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
    [Header("Cast Properties")]
    public float minCastTime = 0.5f;
    
    [Header("Projectile Behaviour")]
    public bool castByPlayer;
    public float speed = 3.0f;
    public float lifeTime = 5.0f;

    public float curveTowardsMouseForce = 1.0f;

    public Vector3 playerPosition;
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

        // Curving towards the mouse if the fireball is thrown
        if (state == FireSpellState.Thrown) CurveTowardMouse();
    }

    // Calls when gameObject collides with another object
    void OnCollisionEnter2D()
    {
        Explode();
    }

    public void Throw()
    {
        // Destroying the fireball if it has not existed for longer than the minimum cast time
        if (timeSinceCreation < minCastTime) 
        {
            Destroy(gameObject, castSpeed);
            animator.SetTrigger("Un-Cast");
            return;
        }
        
        // Adding a force to the fireball to throw it
        state = FireSpellState.Thrown;
        rb.AddForce(transform.up * speed, ForceMode2D.Impulse);
        lifeTime += timeSinceCreation;
    }

    // Handles the fireball explosion
    public void Explode()
    {
        Destroy(gameObject);
    }

    // Function to curve towards the mouse
    void CurveTowardMouse() 
    {
        // Getting the mouse position as a world position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculating the vector direction to point towards the mouse
        Vector2 towardsMouse = mousePosition - transform.position;
        towardsMouse = towardsMouse.normalized;

        // Calculating the distance between the points
        float distanceToMouse = Vector3.Distance(playerPosition, transform.position);

        Debug.Log($"Distance to Mouse: {distanceToMouse}");

        // Adding a force to the fireball
        rb.AddForce(towardsMouse * curveTowardsMouseForce * (1 / distanceToMouse));
    }
}
