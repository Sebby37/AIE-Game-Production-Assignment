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

    [Header("Boss AI Behaviour")]
    public float timeBetweenAttacks = 5.0f;

    [Header("Attack 1 Config")]
    public GameObject fireBall;
    public int castFireBallFrame = 0;
    public Vector2 fireBallCastOffset;
    public int throwFireBallFrame = 15;
    public float fireBallSpeed = 3.5f;

    [Header("Attack 2 Config")]
    public int groundPoundFrame = 8;
    public GameObject slamObject;
    public float slamObjectLifetime = 1.0f;
    public int slamsPerAttack = 3;
    public float timeBetweenSlamSpawns = 0.5f;
    public Vector2 slamSpawnOffset;

    [Header("Attack 3 Config")]
    public int dashBeginFrame = 6;
    public int dashEndFrame = 10;
    public float dashSpeed = 5.0f;
    public int dashTimes = 3;
    public float timeBetweenDashes = 0.5f;

    Animator animator;
    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(AttackTimingCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        // Setting the animation speed variable in the animation controller each frame
        animator.SetFloat("Speed", animationSpeed);

        // Debug keys to trigger attacks
        if (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                Attack(1);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                Attack(2);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                Attack(3);
        }
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

    // Function to attack
    void Attack(int attackNumber)
    {
        // Returning if the current state is not idle
        if (state != GolemStates.Idle)
            return;

        // Setting the state
        state = GolemStates.Attacking;

        // Attacking based on the attack's number
        switch (attackNumber)
        {
            case 1:
                StartCoroutine(Attack1Coroutine());
                break;
            case 2:
                StartCoroutine(Attack2Coroutine());
                break;
            case 3:
                StartCoroutine(Attack3Coroutine());
                break;
            default:
                state = GolemStates.Idle;
                break;
        }
    }

    // The attack timing coroutine
    IEnumerator AttackTimingCoroutine()
    {
        while (true)
        {
            // Getting a random attack to perform
            int chosenAttack = Random.Range(1, 4);

            // Performing the chosen attack
            Attack(chosenAttack);

            // Waiting for the golem boss to be idle before beginning another attack
            while (state != GolemStates.Idle)
            {
                // Golem is attacking
                print(state.ToString());
                yield return null;
            }

            // Waiting to start the next attack
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    // The Attack 1 Coroutine
    IEnumerator Attack1Coroutine()
    {
        // Triggering the animation
        animator.SetTrigger("Attack 1");

        // Waiting until the fireball needs to be cast
        float timeToWait = ((float)castFireBallFrame / (float)animationSampleRate) * (1 / animationSpeed);
        yield return new WaitForSeconds(timeToWait);

        // Instantiating the fireball
        GameObject newFireBall = Instantiate(fireBall, (Vector2) transform.position + fireBallCastOffset, Quaternion.identity);

        // Waiting until the fireball is to be thrown
        timeToWait = ((float)(throwFireBallFrame - castFireBallFrame) / (float)animationSampleRate) * (1 / animationSpeed);
        yield return new WaitForSeconds(timeToWait);

        // Making sure the fireball ignores collision with the boss
        Physics2D.IgnoreCollision(newFireBall.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        
        // Throwing the fireball
        BossFireBall fireBallComponent = newFireBall.GetComponent<BossFireBall>();
        fireBallComponent.Throw(fireBallSpeed, gameObject);

        // Setting the attack state to idle as the attack is complete
        state = GolemStates.Idle;
    }

    // The Attack 2 Coroutine
    IEnumerator Attack2Coroutine()
    {
        // Triggering the animation
        animator.SetTrigger("Attack 2");

        // Waiting until the ground pound is to happen
        float timeToWait = ((float) groundPoundFrame / (float) animationSampleRate) * (1 / animationSpeed);
        yield return new WaitForSeconds(timeToWait);

        // Getting the player's GameObject to spawn slam attacks on them
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Spawning slam objects
        for (int i = 0; i < slamsPerAttack; i++)
        {
            // Instantiating the object at the player's position
            GameObject currentSlam = Instantiate(slamObject, (Vector2) player.transform.position + slamSpawnOffset, Quaternion.identity);

            // Setting the slam object to be destroyed after an interval
            Destroy(currentSlam, slamObjectLifetime);

            // Waiting to spawn the next slam object
            if (i < slamsPerAttack - 1)
                yield return new WaitForSeconds(timeBetweenSlamSpawns);
        }
        
        // Setting the attack state to idle as the attack is complete
        state = GolemStates.Idle;
    }

    // The Attack 3 Coroutine
    IEnumerator Attack3Coroutine()
    {
        for (int i = 0; i < dashTimes; i++)
        {
            // Triggering the animation
            animator.SetTrigger("Attack 3");

            // Waiting until the beginning of the attack behaviour
            float timeToWait = ((float)dashBeginFrame / (float)animationSampleRate) * (1 / animationSpeed);
            yield return new WaitForSeconds(timeToWait);

            // Finding the direction to the player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector2 directionToPlayer = (((Vector2)player.transform.position + slamSpawnOffset) - (Vector2)transform.position).normalized;

            // Setting the golem's velocity
            rb.velocity = directionToPlayer * dashSpeed;

            // Waiting until the end of the dash behaviour
            timeToWait = ((float)(dashEndFrame - dashBeginFrame) / (float)animationSampleRate) * (1 / animationSpeed);
            yield return new WaitForSeconds(timeToWait);

            // Resetting the velocity to zero as the dash is complete
            rb.velocity = Vector2.zero;

            // Setting the attack state to idle as the attack is complete
            state = GolemStates.Idle;

            // Delaying for the next dash
            if (i < dashTimes - 1)
                yield return new WaitForSeconds(timeBetweenDashes);
        }
    }
}
