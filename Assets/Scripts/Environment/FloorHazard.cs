using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// created by Joseph
public class FloorHazard : MonoBehaviour
{
    // GameObject collider
    Collider2D hazardCollider;
    // ACtivates when script is started
    private void Start()
    {
        hazardCollider = GetComponent<Collider2D>();
    }

    // Activates when player enters and stays within the trigger
    void OnTriggerStay2D(Collider2D collision)
    {
        // Checks if the gameobject collider is the player
        if (collision.gameObject.tag == "Player")
        {
            // Accesses PlayerMovement component and checks if the current player state is rolling
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement.playerState != PlayerMovementStates.Rolling)
            {
                // Respawn Player Here
                collision.gameObject.transform.position = Vector2.zero;
                print("Reset Player");
            }
        }
    }
}
