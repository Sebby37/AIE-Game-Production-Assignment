using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [Header("Leap Settings")]
    public bool animationIsLeaping = false;
    public float idleLeapDistance = 1.5f;
    public float pursueLeapDistance = 3.0f;

    // Local leap variables
    float leapTime;
    Vector2 leapDirection;
    float leapVelocity;
    
    // Components
    Rigidbody2D rb;
    Animator animator;
    GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        // Calculating leap variables
        leapTime = 38f / 60f;

        // Getting the components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Checking whether the leaping animation is playing
        animationIsLeaping = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Jump Airborne";

        // Setting velocity
        rb.velocity = animationIsLeaping ? leapDirection * leapVelocity : Vector2.zero;
        
        // TESTING PURPOSES - Leaping in the direction of the mouse pointer
        if (Input.GetMouseButtonDown(0)) Leap(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    // Function to trigger a leap
    void Leap(Vector2 leapPosition)
    {
        // Returning if a leap is already in progress
        if (animationIsLeaping) return;
        
        // Triggering the jump animation code
        animator.SetTrigger("Jump");

        // Calculating the leap direction and velocity
        leapVelocity = pursueLeapDistance / leapTime;
        leapDirection = (leapPosition - (Vector2)transform.position).normalized;
    }
}
