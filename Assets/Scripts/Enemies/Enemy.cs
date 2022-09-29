using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Written by Sebastian Cramond
/*
  How to 
 [System.Serializable]
 public class TestCategory
 {
     public string name;
 }
 public TestCategory coolTest;
*/

public enum EnemyState
{
    Idle,
    Pursuit,
    Attacking,
    Damaged,
    Dying
}

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float health;
    public float aggroRange;

    float maxHealth;
    GameObject playerObject;
    [SerializeField] EnemyState state = EnemyState.Idle;

    [Header("Idle Behaviour")]
    public float timeBetweenIdleBehaviour;
    public UnityEvent idleBehaviour;

    [Header("Player Pursuit Behaviour")]
    public float timeBetweenPlayerPursuit;
    public UnityEvent playerPursuitBehaviour;

    [Header("Attack Behaviour")]
    public float timeBetweenAttacks;
    public UnityEvent attackBehaviour;

    [Header("Damage Behaviour")]
    public UnityEvent damageBehaviour;

    [Header("Death Behaviour")]
    public UnityEvent deathBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, playerObject.transform.position) <= aggroRange)
        {
            state = EnemyState.Pursuit;
        }
    }
}
