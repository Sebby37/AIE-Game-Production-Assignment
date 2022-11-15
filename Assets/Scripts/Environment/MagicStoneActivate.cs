using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicStoneActivate : MonoBehaviour
{
    Animator animator;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            animator.SetTrigger("FireActivate");
        }
    }
}
