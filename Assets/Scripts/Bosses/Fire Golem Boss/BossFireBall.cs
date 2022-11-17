using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Sebastian Cramond
public class BossFireBall : MonoBehaviour
{
    [Header("Config")]
    public float bossDamage = 5.0f;
    public GameObject spawnOnCollision;
    public GameObject caster;

    bool hasBeenCast = false;
    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function to throw the fireball
    public void Throw(float speed, GameObject _caster)
    {
        // Setting the caster
        caster = _caster;
        
        // Getting the player GameObject
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Getting the direction to the player
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;

        // Making sure the rigidbody is not null
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        // Setting the velocity of the fireball
        rb.velocity = directionToPlayer * speed;

        // Setting the fireball to have been cast
        hasBeenCast = true;
    }

    // For sword slash handling
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the fireball is hit with the sword, it is reflected and collisions with the boss are reallowed
        if (hasBeenCast && collision.CompareTag("Player Damager"))
        {
            rb.velocity *= -1.0f;
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), caster.GetComponent<Collider2D>(), false);
        }
    }

    // For collision handling
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasBeenCast)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // Damage enemy
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                // Damage player
            }
            else
            {
                // Create a fire sprite at the fireball's position
                Instantiate(spawnOnCollision, transform.position, Quaternion.identity);
            }

            // Destroy the fireball
            Destroy(gameObject);
        }
    }
}
