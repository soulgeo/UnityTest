using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenuUI;
    public GameObject mainMenuUI;
    public GameObject credits;

    AudioManager audioManager;
    public AudioMixer audioMixer;

    bool inSubMenu = false;

    void Awake()
    {
        switch (PlayerPrefs.GetInt("WindowMode"))
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            default:
                break;
        }

        int screenWidth = PlayerPrefs.GetInt("ScreenWidth");
        int screenHeight = PlayerPrefs.GetInt("ScreenHeight");
        Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreenMode);

        float volume = AudioManager.LinearToDecibel(PlayerPrefs.GetFloat("Volume", 0.75f));
        audioMixer.SetFloat("masterVolume", volume);
        Debug.Log("WindowMode: " + PlayerPrefs.GetInt("WindowMode") + ", ScreenResolution: " + screenWidth + " x " + screenHeight + ", Volume: " + volume + ".");

        audioManager = FindObjectOfType<AudioManager>();

        optionsMenuUI.SetActive(false);
        mainMenuUI.SetActive(true);

        credits.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && inSubMenu)
        {
            Back();
        }
    }

    public void LatePlay()
    {
        audioManager.Play("Select");
        Invoke("Play", 0.5f);
    }

    void Play()
    {
        SceneManager.LoadScene("Scene01");
    }

    public void Options()
    {
        audioManager.Play("Select");
        mainMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
        inSubMenu = true;
    }

    public void Credits()
    {
        audioManager.Play("Select");
        mainMenuUI.SetActive(false);
        credits.SetActive(true);
        inSubMenu = true;
    }

    public void LateQuit()
    {
        audioManager.Play("Select");
        Invoke("Quit", 0.5f);
    }

    public void Back()
    {
        audioManager.Play("Select");
        mainMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        credits.SetActive(false);
        inSubMenu = false;
    }

    void Quit()
    {
        Application.Quit();
    }
}
