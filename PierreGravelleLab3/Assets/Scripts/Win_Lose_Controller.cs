using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Win_Lose_Controller : MonoBehaviour
{
    static public  List<GameObject> playerUnits;
    static public List<GameObject> enemyUnits;

    bool mainMenuButtonPressed;

    // Use this for initialization
    void Start()
    {
        mainMenuButtonPressed = false;
        playerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit").ToList<GameObject>();
        enemyUnits = GameObject.FindGameObjectsWithTag("Enemy").ToList<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "Lab3")
        {
            if(playerUnits.Count == 0)
            {
                Lose();
            }
            else if (enemyUnits.Count == 0)
            {
                Win();
            }
        }

        if (mainMenuButtonPressed && !GetComponent<AudioSource>().isPlaying)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    void Win()
    {
        SceneManager.LoadScene("WinScreen");
    }

    void Lose()
    {
        SceneManager.LoadScene("LoseScreen");
    }

    public void BackToMainMenu()
    {
        mainMenuButtonPressed = true;
        GetComponent<AudioSource>().Play();
    }
}
