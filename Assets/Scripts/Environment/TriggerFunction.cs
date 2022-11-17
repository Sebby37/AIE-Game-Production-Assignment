using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerFunction : MonoBehaviour
{
    [Header("Tags")]
    public List<string> triggerTags = new List<string>();

    [Header("Events")]
    public UnityEvent onEnterEvent;
    public UnityEvent onStayEvent;
    public UnityEvent onExitEvent;

    // The onEnter function
    void Enter(GameObject collision)
    {
        // Running the event if the collider's tag is in the list of tags
        if (triggerTags.Contains(collision.tag))
        {
            // Triggering the event
            if (onEnterEvent != null)
                onEnterEvent.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enter(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enter(collision.gameObject);
    }

    // The onStay function
    void Stay(GameObject collision)
    {
        // Running the event if the collider's tag is in the list of tags
        if (triggerTags.Contains(collision.tag))
        {
            // Triggering the event
            if (onStayEvent != null)
                onEnterEvent.Invoke();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Stay(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Stay(collision.gameObject);
    }

    // The onExit function
    void Exit(GameObject collision)
    {
        // Running the event if the collider's tag is in the list of tags
        if (triggerTags.Contains(collision.tag))
        {
            // Triggering the event
            if (onExitEvent != null)
                onEnterEvent.Invoke();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Exit(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Exit(collision.gameObject);
    }

    // A function to destroy the gameobject
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
