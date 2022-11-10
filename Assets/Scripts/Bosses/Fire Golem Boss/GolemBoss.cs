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

    Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Setting the animation speed variable in the animation controller each frame
        animator.SetFloat("Speed", animationSpeed);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            Attack2();
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
}
