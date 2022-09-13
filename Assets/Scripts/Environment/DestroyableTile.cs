using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Written by Sebastian Cramond
public class DestroyableTile : MonoBehaviour
{
    [Header("Ways To Be Destroyed")]
    public bool burnable = false;
    public bool slashable = true;

    [Header("Animation Properties")]
    public Animator animator;
    public float animationTime = 0.0f;
    public string destroyAnimationTrigger;

    [Header("Destruction Properties")]
    public GameObject droppedItem;
    [Range(0, 1)] public float dropProbability = 0.5f;
    public UnityEvent destructionEvent;

    // OnTriggerStay is currently used only for player sword slash
    private void OnTriggerStay2D(Collider2D collision)
    {
        // If this tile can be destroyed using a slash and the collider is a player slash, the tile is destroyed
        if (slashable && collision.CompareTag("Player Damager")) DestroyTile();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If this tile can be burned and the collision was with a fireball, the tile is destroyed
        if (burnable && collision.gameObject.CompareTag("Fire Ball")) DestroyTile();
    }

    // Function to destroy the current tile
    void DestroyTile()
    {
        // Playing the destruction animation
        if (animator != null) animator.SetTrigger(destroyAnimationTrigger);

        // Calling the destruction event
        if (destructionEvent != null) destructionEvent.Invoke();

        // Dropping the droppable item based on probability
        if (droppedItem != null)
            if (Random.value <= dropProbability)
                Instantiate(droppedItem, transform.position, transform.rotation);

        // Destroying the GameObject after the animation is completed
        Destroy(gameObject, animationTime);
    }
}
