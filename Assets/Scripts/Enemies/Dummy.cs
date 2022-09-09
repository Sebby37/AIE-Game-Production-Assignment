using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [Header("Health and Invincibility")]
    public float maxHealth = 5;
    public float postHitInvulnerability = 0.25f;

    float health;
    bool invulnerable = false;
    float invulnerabilityTimer = 0;
    
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Invulnerability timing
        if (invulnerable) invulnerabilityTimer += Time.deltaTime;
        if (invulnerabilityTimer >= postHitInvulnerability) invulnerable = false;

        // Setting the color of the sprite to red if it is invulnerable to show that it was hit
        spriteRenderer.color = invulnerable ? Color.red : Color.yellow;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Returning if the dummy is invulnerable
        if (invulnerable) return;

        // Currently there exists a bug where ontriggerstay/enter is called only when moving the collider after disabling/re-enabling it
        Debug.Log(collision.gameObject.name);

        // If the dummy is in a player damager trigger
        if (collision.CompareTag("Player Damager"))
        {
            Damage(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Returning if the dummy is already invulnerable
        if (invulnerable) return;
        
        // If the dummy was hit by a fireball
        if (collision.gameObject.CompareTag("Fire Ball"))
        {
            // Getting the FireSpell component and damaging the dummy based on it's damage variable
            FireSpell fireBall = collision.gameObject.GetComponent<FireSpell>();
            if (fireBall != null && fireBall.castByPlayer) Damage(fireBall.damage);
        }
    }

    // Function to take damage
    void Damage(float amountOfDamage)
    {
        health -= amountOfDamage;
        invulnerable = true;
        invulnerabilityTimer = 0;
    }
}
