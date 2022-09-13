using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Sebastian Cramond
public class SimpleFireballAttack : MonoBehaviour
{
    public float fireballSpeedMultiplier = 0.5f;
    public float activeDistance = 4.5f;
    public GameObject fireBallPrefab;
    GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Attack()
    {
        // Returning if the player is null
        if (player == null) return;

        // Returning if the player is not within the active distance
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer > activeDistance) return;

        // Getting the angle needed to launch towards the player
        Vector2 playerPos = (Vector2)player.transform.position;
        Vector2 direction = (playerPos - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float offset = 90f;

        // Setting the rotation of the enemy to the calculated angle
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle - offset));
        
        // Creating a fireball object
        GameObject fireBall = Instantiate(fireBallPrefab, transform.position + transform.up, transform.rotation);
        FireSpell fireComponent = fireBall.GetComponent<FireSpell>();

        // Throwing the fireball and setting some variables
        fireComponent.playerPosition = player.transform.position;
        fireComponent.speed *= fireballSpeedMultiplier;
        fireComponent.Throw(GetComponent<Collider2D>());
    }
}
