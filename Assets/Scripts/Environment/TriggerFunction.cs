using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerFunction : MonoBehaviour
{
    [Header("Config")]
    public UnityEvent eventToTrigger;
    public bool destroyOnTrigger = true;
    public List<string> triggerTags = new List<string>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Running the event if the collider's tag is in the list of tags
        if (triggerTags.Contains(collision.tag))
        {
            // Triggering the event
            if (eventToTrigger != null)
                eventToTrigger.Invoke();

            // Destroying the trigger gameobject if set to do so
            if (destroyOnTrigger)
                Destroy(gameObject);
        }
    }
}
