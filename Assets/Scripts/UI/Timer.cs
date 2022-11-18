using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    //Script by Toby McDonald

    public Text m_timer;
    float m_gameTime = 0;
    private string finalTime;
    PlayerHealth playerHealth;
    public Text m_finalTimer;
    private CanvasGroup m_canvasGroup;
    public Text FinalEnemiesKilled;
    private bool checkOnce;
    int totalenemiesdead = 0;
    ArenaSpawner enemies;
    public GameObject spawner;

    // Start is called before the first frame update
    void Start()
    {

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            print("Error, couldnt find playerhealth");
        }

        m_finalTimer.text = "";
        FinalEnemiesKilled.text = "";


        enemies = spawner.GetComponent<ArenaSpawner>();

    if (enemies == null)
        {
            print("Cannot find enemies killed");
        }

    } 

    // Update is called once per frame
    void Update()
    {

        m_gameTime += Time.deltaTime;
        int seconds = Mathf.RoundToInt(m_gameTime);
        m_timer.text = string.Format("{0:D2}:{1:D2}", (seconds / 60), (seconds % 60));

         
        
        if (playerHealth.isDead)
        {
            print("MOving timer");
            endGame();

        }
        

    }

    void endGame()
    {

        if (!checkOnce)
        {
            m_finalTimer.text = m_timer.text;
           
            checkOnce = true;

        }

        m_timer.text = "";

        totalenemiesdead = enemies.enemiesKilled;

        FinalEnemiesKilled.text = "You killed " + totalenemiesdead + " enemies!";

    }

}
