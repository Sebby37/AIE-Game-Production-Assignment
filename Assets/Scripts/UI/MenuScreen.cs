using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScreen : MonoBehaviour
{

    public Button startButton;
    public Button optionsButton;
    public Button exitButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void beginGame()
    {

        SceneManager.LoadScene("CombatTest");

    }

    public void exitGame()
    {

        Application.Quit();

    }

}
