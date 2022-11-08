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

    public float currentMana;
    public float maxMana;
    public Image m_manaFill;

    public Image m_healthBar;
    public Image m_healthFrame;
    public Sprite m_startFrame;
    public Sprite m_endFrame;

    private float currentPotion;
    public float maxPotion;
    public Image m_potionFill;

    public Text currencyText;

    private float moneyCount;

    // Start is called before the first frame update
    void Start()
    {

        currentHealth = maxHealth;
        currentPotion = maxPotion;
        currentMana = maxMana;

    }

    // Update is called once per frame
    void Update()
    {

        UpdateHealthUI();
        HealPlayer();

        /*if (Input.GetKeyDown(KeyCode.L))
        {

            currentHealth -= 10;

        }*/

        currentMana += 1.0f * Time.deltaTime;
        UpdateManaUI();

        /*if (Input.GetKeyDown(KeyCode.M))
        {

            currentMana -= 20;
            print(currentMana);
            UpdateManaUI();

        }*/

        if (m_healthBar.fillAmount < 0.25)
        {

            m_healthFrame.sprite = m_endFrame;

        }
        else
        {

            m_healthFrame.sprite = m_startFrame;

        }

        if (currentMana > 100)
        {

            currentMana = 100;
            UpdateManaUI();

        }


        PlayerMovement playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();//gameObject.GetComponent<PlayerMovement>();
        GameObject fireSpell = playerMovement.fireSpell;//.GetComponent<FireSpell>();

        if (playerMovement.playerState == PlayerMovementStates.Casting && fireSpell != null)
        {
            currentMana -= 20 * Time.deltaTime;
            UpdateManaUI();
        }

        if (currentHealth <= 0)
        {
            Death();
        }

    }


    public void UpdateManaUI()
    {

        m_manaFill.fillAmount = currentMana / maxMana;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {




        /* DO LATER IF THERE IS TIME (THERE WILL NOT BE) 8/11/2022 - If you are marking this I am sorry ~UwU~
         * 
         * 
         * Coin coin = collision.gameObject.GetComponent<Coin>();

        if (coin != null)
        {
            if (coin.gameObject.name == "BronzeCoin")
            {

                moneyCount += 5;

                currencyText.text = "$" + moneyCount;

                Destroy(gameObject);

            }

            if (coin.gameObject.name == "SilverCoin")
            {

                moneyCount += 10;

                currencyText.text = "$" + moneyCount;

                Destroy(gameObject);

            }

            if (coin.gameObject.name == "GoldCoin")
            {

                moneyCount += 15;

                currencyText.text = "$" + moneyCount;

                Destroy(gameObject);

            }
        }*/

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.name.Contains("Slime"))
        {

            currentHealth -= 10;
            UpdateHealthUI();

        }



        if (collision.gameObject.CompareTag("Fire Ball"))
        {

            FireSpell fireSpell = collision.gameObject.GetComponent<FireSpell>();

            if (fireSpell != null && !fireSpell.castByPlayer)
            {
                currentHealth -= 5;
                UpdateHealthUI();
            }

        }
    }

    void Death()
    {

        PlayerMovement playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        playerMovement.playerAnimator.SetTrigger("Die");

        

    }

}
