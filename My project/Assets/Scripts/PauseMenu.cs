using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public static bool inOptions = false;
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject guideUI;
    public AudioManager audioManager;
    
    public float pauseFade = 0.05f;
    public float resumeFade = 0.5f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        guideUI.SetActive(false);

        if (GameMemory.firstPlaythrough)
        {
            guideUI.SetActive(true);
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            gameIsPaused = true;
            audioManager.setMixerToPaused();
            Time.timeScale = 0;

            GameMemory.firstPlaythrough = false;
        }
    }

    void Update()
    {
        if (Keyboard.current[Key.F1].wasPressedThisFrame)
        {
            if (!gameIsPaused || inOptions) 
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    public void LoadMenu()
    {
        audioManager.Play("Select");
        Time.timeScale = 1;
        gameIsPaused = false;
        audioManager.setMixerToPlaying();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        audioManager.Play("Select");
        Application.Quit();
    }

    public void ViewGuide()
    {
        audioManager.Play("Select");
        audioManager.setMixerToPaused();
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        guideUI.SetActive(true);
        gameIsPaused = true;
        Time.timeScale = 0;
    }

    public void OpenOptions()
    {
        audioManager.Play("Select");
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
        inOptions = true;
    }

    

    public void Pause()
    {
        audioManager.Play("Select");
        audioManager.setMixerToPaused();
        //GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        pauseMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        guideUI.SetActive(false);
        gameIsPaused = true;
        Time.timeScale = 0;
        inOptions = false;
        Cursor.visible = true;
    }

    public void Resume()
    {
        audioManager.Play("Select");
        Time.timeScale = 1;
        audioManager.setMixerToPlaying(resumeFade);
        //GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        pauseMenuUI.SetActive(false);
        guideUI.SetActive(false);
        gameIsPaused = false;
        Cursor.visible = false;
    }


    private IEnumerator SlowPause(float time)
    {
        float startTimeScale = Time.timeScale;
        float elapsed = 0f;
        while (elapsed < time)
        {
            Time.timeScale = Mathf.Lerp(startTimeScale, 0, elapsed / time);
            elapsed += Time.unscaledDeltaTime;
            if (!gameIsPaused) yield break;
            else yield return null;
        }
        Time.timeScale = 0;

    }

    private IEnumerator SlowResume(float time)
    {
        float startTimeScale = Time.timeScale;
        float elapsed = 0f;
        while (elapsed < time)
        {
            Time.timeScale = Mathf.Lerp(startTimeScale, 1, elapsed / time);
            elapsed += Time.unscaledDeltaTime;
            if (gameIsPaused) yield break;
            else yield return null;
        }
        Time.timeScale = 1;
    }

    

}
