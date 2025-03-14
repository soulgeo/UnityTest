using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public bool gameHasEnded = false;
    public float restartDelay = 2f;
    public AudioMixer audioMixer;
    public AudioManager audioManager;

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

    }

    private void Start()
    {
        audioManager.Play("Soundtrack");
    }


    public void GameOver()
    {
        if (!gameHasEnded)
        {
            gameHasEnded = true;
            Invoke("Restart", restartDelay);
        }
    }

    public void Retry()
    {
        Invoke("Restart", 0.5f);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator SlowQuit(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Application.Quit();
    }
}
