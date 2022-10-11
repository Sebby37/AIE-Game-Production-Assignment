using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// created by Joseph
public class FloorHazard : MonoBehaviour
{
    Collider2D hazardCollider;
    private void Start()
    {
        hazardCollider = GetComponent<Collider2D>();
    }

    // Activates when player enters trigger
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Checks if the gameobject collider is the player
        if (collision.gameObject.tag == "Player")
        {
            // Accesses PlayerMovement component and checks if the current player state is rolling
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement.playerState == PlayerMovementStates.Rolling)
            {
                // Respawn Player Here
                // Place holder script to indicate that plyaer has hit spiketrap
                hazardCollider.isTrigger = true;
            }
        }
    }

    IEnumerator RollColliderReset(float rollTime)
    {
        yield return new WaitForSeconds(rollTime);
        hazardCollider.isTrigger = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hazardCollider.isTrigger = false;
    }
    private void Update()
    {
        print(hazardCollider.isTrigger);
    }
}
