using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehaviour : MonoBehaviour
{
    public float aggroRange = 5.0f;

    // Leaping
    bool leaping = false;
    float leapTimer = 0.0f;
    float leapVelocity = 0.0f;
    Vector3 leapTargetVector;

    // Components and such
    Rigidbody2D rb;
    Animator animator;
    GameObject player;

    // Start runs on start
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update runs on update
    void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < aggroRange) PursuePlayer();
    }

    // The idle behaviour of the Slime
    public void Idle()
    {

    }
    
    // The behaviour to pursue the player
    public void PursuePlayer()
    {
        Leap(player.transform.position, 1.5f);
    }

    // The behaviour when damaged
    public void Damaged()
    {

    }

    // The death behaviour
    public void Die()
    {

    }

    // A function to leap to a position
    void Leap(Vector2 landingPosition, float leapTime)
    {
        // TODO: Split jump animation into prepare, in air, land ||| Improve leaping code!!! (Add delay between leaps)
        if (!leaping)
        {
            leaping = true;
            leapTimer = 0.0f;
            animator.SetTrigger("Jump");
            leapTargetVector = (landingPosition - (Vector2) transform.position).normalized;
            leapVelocity = Vector3.Distance(landingPosition, transform.position);
            rb.velocity = leapTargetVector * leapVelocity;
        }
        else
        {
            if (leapTimer >= leapTime)
            {
                leaping = false;
                rb.velocity = Vector3.zero;
            }

            leapTimer += Time.deltaTime;
        }
    }
}
