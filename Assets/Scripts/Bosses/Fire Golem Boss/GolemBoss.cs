using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GolemStates
{
    Intro,
    Idle,
    Dead,
    Jumping,
    Attacking
}

public class GolemBoss : MonoBehaviour
{
    [Header("State Information")]
    public GolemStates state;
    public bool inBossBattle = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function to begin the boss battle
}
