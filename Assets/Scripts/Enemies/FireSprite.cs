using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSprite : MonoBehaviour
{
    [Header("Movement Settings")]
    public float pursuitDistance = 5.0f;
    public float pursuitSpeed = 1.0f;

    Vector2 directionToPlayer;
    bool pursuingPlayer = false;

    [Header("Attack Settings")]
    public GameObject fireballPrefab;
    public float timeBetweenAttacks = 3.5f;
    public float fireballSpeedMultiplier = 0.5f;

    float attackTimer = 0.0f;

    Rigidbody2D rb;
    Animator animator;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        // Getting components and objects
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Setting the pursuing player bool
        pursuingPlayer = Vector2.Distance(transform.position, player.transform.position) <= pursuitDistance;

        // Getting the direction to the player (This is zero if the player is not being pursued
        if (pursuingPlayer)
            directionToPlayer = (player.transform.position - transform.position).normalized;
        else
            directionToPlayer = Vector2.zero;
        
        // Moving towards the player
        rb.velocity = directionToPlayer * pursuitSpeed;

        // Incrementing the attack timer
        attackTimer += pursuingPlayer ? Time.deltaTime : 0.0f;

        // Throwing a fireball if the attack timer has exceeded the time between attacks
        if (attackTimer >= timeBetweenAttacks)
            AttackPlayer();
    }

    // Function to attack the player
    [ContextMenu("Attack Player")]
    void AttackPlayer()
    {
        // Returning if the fireball is not attacking and the distance to player is zero
        if (!pursuingPlayer || directionToPlayer == Vector2.zero)
            return;

        // Playing the attack animation
        animator.SetTrigger("Attack");

        // Resetting the attack timer
        attackTimer = 0.0f;

        // Beginning the attack coroutine
        StartCoroutine(AttackAfterDelay());
    }

    // Coroutine to throw an attack after a delay for animation
    IEnumerator AttackAfterDelay()
    {
        yield return new WaitForSeconds((13.0f / 60.0f) * (1.0f / 0.3f));

        // Creating the fireball
        GameObject fireball = Instantiate(fireballPrefab);
        FireSpell fireComponent = fireball.GetComponent<FireSpell>();
        fireball.transform.position = transform.position;

        // Throwing the fireball and setting some variables
        fireComponent.playerPosition = player.transform.position;
        fireComponent.speed *= fireballSpeedMultiplier;
        fireComponent.Throw(GetComponent<Collider2D>(), directionToPlayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = pursuingPlayer ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitDistance);
    }
}
