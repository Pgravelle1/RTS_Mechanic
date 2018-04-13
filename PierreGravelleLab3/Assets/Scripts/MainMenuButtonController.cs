using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtonController : MonoBehaviour
{
    [SerializeField]
    GameObject menuPanel, optionsPanel, controlsPanel;

    [SerializeField]
    Slider sfxSlider, musicSlider;

    [SerializeField]
    VolumeMixer volMix;

    AudioSource myAudio;

    // Use this for initialization
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", sfxSlider.value);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", musicSlider.value);
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = true;
    }

    public void ButtonEvent(string btnNum)
    {
        btnNum = btnNum.ToUpper();
        myAudio.Play();

        switch (btnNum)
        {
            case "PLAY":
                Time.timeScale = 1;
                SceneManager.LoadScene("Lab3");
                break;
            case "OPTIONS":
                menuPanel.SetActive(false);
                controlsPanel.SetActive(false);
                optionsPanel.SetActive(true);
                break;
            case "OPTIONS BACK":
                menuPanel.SetActive(true);
                optionsPanel.SetActive(false);
                controlsPanel.SetActive(false);
                break;
            case "CONTROLS":
                menuPanel.SetActive(false);
                optionsPanel.SetActive(false);
                controlsPanel.SetActive(true);
                break;
            case "CONTROLS BACK":
                menuPanel.SetActive(false);
                optionsPanel.SetActive(true);
                controlsPanel.SetActive(false);
                break;
            case "QUIT":
                Application.Quit();
                break;
            default:
                break;
        }
    }
}
