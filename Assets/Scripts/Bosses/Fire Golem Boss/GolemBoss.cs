using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GolemStates
{
    Intro,
    Idle,
    Dead,
    Jumping,
    Attacking
}

public class GolemBoss : MonoBehaviour
{
    [Header("State Information")]
    public GolemStates state = GolemStates.Idle;
    public bool inBossBattle = false;

    [Header("Animation Config")]
    public float animationSpeed = 1.0f;
    public int animationSampleRate = 24;

    [Header("Attack 2 Config")]
    public int groundPoundFrame = 8;
    public GameObject slamObject;
    public float slamObjectLifetime = 1.0f;
    public int slamsPerAttack = 3;
    public float timeBetweenSlamSpawns = 0.5f;
    public Vector3 slamSpawnOffset;

    [Header("Attack 3 Config")]
    public int dashBeginFrame = 6;
    public int dashEndFrame = 10;
    public float dashSpeed = 5.0f;

    Animator animator;
    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Setting the animation speed variable in the animation controller each frame
        animator.SetFloat("Speed", animationSpeed);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Attack1();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            Attack2();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            Attack3();
    }

    // Function to begin Attack 1
    [ContextMenu("Intro")]
    void Intro()
    {
        // Setting the state
        state = GolemStates.Intro;

        // Triggering the animation
        animator.SetTrigger("Intro");
    }

    // Function to begin Attack 1
    [ContextMenu("Attack 1")]
    void Attack1()
    {
        // Returning if the current state is not idle
        if (state != GolemStates.Idle)
            return;

        // Setting the state
        state = GolemStates.Attacking;

        // Triggering the animation
        animator.SetTrigger("Attack 1");
    }

    // Function to begin Attack 2 - Ground Pound
    [ContextMenu("Attack 2")]
    void Attack2()
    {
        // Returning if the current state is not idle
        if (state != GolemStates.Idle)
            return;
        
        // Setting the state
        state = GolemStates.Attacking;

        // Triggering the animation
        animator.SetTrigger("Attack 2");

        // Beginning the attack coroutine
        StartCoroutine(Attack2Coroutine());
    }

    IEnumerator Attack2Coroutine()
    {
        // Waiting until the ground pound is to happen
        float timeToWait = ((float) groundPoundFrame / (float) animationSampleRate) * (1 / animationSpeed);
        yield return new WaitForSeconds(timeToWait);

        // Getting the player's GameObject to spawn slam attacks on them
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Spawning slam objects
        for (int i = 0; i < slamsPerAttack; i++)
        {
            // Instantiating the object at the player's position
            GameObject currentSlam = Instantiate(slamObject, player.transform.position + slamSpawnOffset, Quaternion.identity);

            // Setting the slam object to be destroyed after an interval
            Destroy(currentSlam, slamObjectLifetime);

            // Waiting to spawn the next slam object
            if (i < slamsPerAttack - 1)
                yield return new WaitForSeconds(timeBetweenSlamSpawns);
        }
        
        // Setting the attack state to idle as the attack is complete
        state = GolemStates.Idle;
    }

    // Function to begin Attack 3
    [ContextMenu("Attack 3")]
    void Attack3()
    {
        // Returning if the current state is not idle
        if (state != GolemStates.Idle)
            return;

        // Setting the state
        state = GolemStates.Attacking;

        // Triggering the animation
        animator.SetTrigger("Attack 3");

        // Beginning the attack coroutine
        StartCoroutine(Attack3Coroutine());
    }

    IEnumerator Attack3Coroutine()
    {
        // Waiting until the beginning of the attack behaviour
        float timeToWait = ((float)dashBeginFrame / (float)animationSampleRate) * (1 / animationSpeed);
        yield return new WaitForSeconds(timeToWait);

        // Finding the direction to the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector2 directionToPlayer = ((player.transform.position + slamSpawnOffset) - transform.position).normalized;

        // Setting the golem's velocity
        rb.velocity = directionToPlayer * dashSpeed;

        // Waiting until the end of the dash behaviour
        timeToWait = ((float)(dashEndFrame - dashBeginFrame) / (float)animationSampleRate) * (1 / animationSpeed);
        yield return new WaitForSeconds(timeToWait);

        // Resetting the velocity to zero as the dash is complete
        rb.velocity = Vector2.zero;

        // Setting the attack state to idle as the attack is complete
        state = GolemStates.Idle;
    }
}
