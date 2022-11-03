using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

public class PlayerHealth : MonoBehaviour
{

    //Script by Toby McDonald

    private float currentHealth;
    public float maxHealth;

    public Image m_healthBar;
    public Image m_healthFrame;
    public Sprite m_startFrame;
    public Sprite m_endFrame;

    private float currentPotion;
    public float maxPotion;
    public Image m_potionFill;

    // Start is called before the first frame update
    void Start()
    {

        currentHealth = maxHealth;
        currentPotion = maxPotion;

    }

    // Update is called once per frame
    void Update()
    {

        UpdateHealthUI();
        HealPlayer();

        if (Input.GetKeyDown(KeyCode.L))
        {

            currentHealth -= 10;

        }

        if (m_healthBar.fillAmount < 0.25)
        {

            m_healthFrame.sprite = m_endFrame;

        }
        else
        {

            m_healthFrame.sprite = m_startFrame;

        }


        /*Coin coin = Collision.gameObject.GetComponent<Coin>();
        if (coin.gameObject.name == "BronzeCoin")
        {



        }*/

    }

    public void UpdateHealthUI()
    {

        m_healthBar.fillAmount = currentHealth / maxHealth;

    }

    public void HealPlayer()
    {

        if (Input.GetKeyDown(KeyCode.H) && m_potionFill.fillAmount > 0)
        {

            m_potionFill.fillAmount = currentPotion / maxPotion;

            currentHealth += 10;

            if (currentHealth > 100)
            {

                currentHealth = 100;

            }

            currentPotion -= 10;
            UpdateHealthUI();

        }

    }

}
