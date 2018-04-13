using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseController : MonoBehaviour
{
    [SerializeField]
    GameObject pausePanel, mainPanel, optionsPanel, gameUI, controlsPanel;
    [SerializeField]
    Slider sfxSlider, musicSlider;

    [SerializeField]
    AudioMixerSnapshot paused, unpaused;

    AudioSource myAudio;
    bool isPaused = false;

    // Use this for initialization
    void Start()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        myAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isPaused)
            {
                paused.TransitionTo(0.01f);
                isPaused = true;
                Cursor.visible = true;
                Time.timeScale = 0;
                optionsPanel.SetActive(false);
                controlsPanel.SetActive(false);
                gameUI.SetActive(false);
                pausePanel.SetActive(true);
                mainPanel.SetActive(true);
            }
            else
            {
                unpaused.TransitionTo(0.01f);
                isPaused = false;
                Time.timeScale = 1;
                optionsPanel.SetActive(false);
                controlsPanel.SetActive(false);
                gameUI.SetActive(true);
                pausePanel.SetActive(false);
                mainPanel.SetActive(false);
            }
        }
    }

    public void ButtonEvent(string btnNum)
    {
        btnNum = btnNum.ToUpper();
        myAudio.Play();

        switch (btnNum)
        {
            case "CONTINUE":
                isPaused = false;
                unpaused.TransitionTo(0.01f);
                Cursor.visible = false;
                Time.timeScale = 1;
                optionsPanel.SetActive(false);
                controlsPanel.SetActive(false);
                gameUI.SetActive(true);
                pausePanel.SetActive(false);
                mainPanel.SetActive(false);
                break;
            case "OPTIONS":
                mainPanel.SetActive(false);
                controlsPanel.SetActive(false);
                Cursor.visible = true;
                optionsPanel.SetActive(true);
                break;
            case "OPTIONS BACK":
                mainPanel.SetActive(true);
                controlsPanel.SetActive(false);
                Cursor.visible = true;
                optionsPanel.SetActive(false);
                break;
            case "CONTROLS":
                mainPanel.SetActive(false);
                optionsPanel.SetActive(false);
                Cursor.visible = true;
                controlsPanel.SetActive(true);
                break;
            case "CONTROLS BACK":
                mainPanel.SetActive(false);
                controlsPanel.SetActive(false);
                Cursor.visible = true;
                optionsPanel.SetActive(true);
                break;
            case "QUIT":
                Time.timeScale = 1;
                unpaused.TransitionTo(0.01f);
                SceneManager.LoadScene("MainMenu");
                break;
            default:
                break;
        }
    }

}
