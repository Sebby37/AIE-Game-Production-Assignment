using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEngine.SceneManagement;

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

    public bool isDead;
    public float fadeSpeed;

    public GameObject specialText;

    private CanvasGroup deathUI;

    private string lastScene;

    // Start is called before the first frame update
    void Start()
    {

        deathUI = specialText.GetComponent<CanvasGroup>();
        currentHealth = maxHealth;
        currentPotion = maxPotion;
        currentMana = maxMana;
        specialText.SetActive(false);
        isDead = false;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("EDXITUS");
            SceneManager.LoadScene("MenuScreen");

        }

        UpdateHealthUI();
        HealPlayer();

        if (Input.GetKeyDown(KeyCode.L))
        {

            currentHealth -= 10;

        }

        currentMana += 1.0f * Time.deltaTime;
        UpdateManaUI();

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
            isDead = true;
            Death();
        }

        /*if (deathUI.alpha >= 1 && Input.anyKeyDown)
        {

            lastScene = LevelChanger.lastSceneName;
            if (lastScene == null)
            {

                lastScene = "MenuScreen";

            }
            SceneManager.LoadScene(lastScene);


        }*/
        
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

        HealthCrystal healthCrystal = collision.gameObject.GetComponent<HealthCrystal>();

        if (healthCrystal != null)
        {

            Debug.Log("Collect crystal");
            currentPotion += 5;
            m_potionFill.fillAmount = currentPotion / maxPotion;
            Destroy(healthCrystal.gameObject);


        }


        Coin coin = collision.gameObject.GetComponent<Coin>();

        if (coin != null)
        {
            if (coin.gameObject.name.Contains("BronzeCoin"))
            {

                moneyCount += 5;

                currencyText.text = "$" + moneyCount;

                Destroy(coin.gameObject);

            }

            if (coin.gameObject.name.Contains("SilverCoin"))
            {

                moneyCount += 10;

                currencyText.text = "$" + moneyCount;

                Destroy(coin.gameObject);

            }

            if (coin.gameObject.name.Contains("GoldCoin"))
            {

                moneyCount += 15;

                currencyText.text = "$" + moneyCount;

                Destroy(coin.gameObject);

            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //Take Damage from Slime
        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.name.Contains("Slime"))
        {

            currentHealth -= 10;
            UpdateHealthUI();

        }

        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.name.Contains("Golem"))
        {

            currentHealth -= 15;
            UpdateHealthUI();

        }

        //Take Damage from Boss
        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.name.Contains("Boss"))
        {

            currentHealth -= 20;
            UpdateHealthUI();

        }

        //Take Damage from Fireball
        if (collision.gameObject.CompareTag("Fire Ball"))
        {

            FireSpell fireSpell = collision.gameObject.GetComponent<FireSpell>();
            BossFireBall bossFireBall = collision.gameObject.GetComponent<BossFireBall>();

            if (fireSpell != null && !fireSpell.castByPlayer)
            {
                currentHealth -= 15;
                UpdateHealthUI();
            }

            if (bossFireBall != null)
            {

                currentHealth -= 22;
                UpdateHealthUI();

            }

        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        //Take Damage from ground thing
        if (collision.gameObject.CompareTag("Damages Player"))
        {

            currentHealth -= 10 * Time.deltaTime;
            UpdateHealthUI();

        }

    }

    public void Death()
    {       

        if (isDead == true)
        {

            deathUI = specialText.GetComponent<CanvasGroup>();

            Debug.Log("Dying");

            specialText.SetActive(true);

            gameObject.GetComponent<PlayerMovement>().Die();

            if (deathUI.alpha < 1)
            {
                deathUI.alpha += 0.5f * fadeSpeed * Time.deltaTime;               

            }            

            /*Color objectColour = deathText.GetComponent<CanvasRenderer>();
            float fadeAmount = objectColour.a + (fadeSpeed * Time.deltaTime);

            objectColour = new Color(objectColour.r, objectColour.g, objectColour.b, fadeAmount);
            deathText.GetComponent<CanvasRenderer>().material.color = objectColour;*/

            

        }

    }

    public void Respawn()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void Menu()
    {

        SceneManager.LoadScene("MenuScreen");

    }

}
