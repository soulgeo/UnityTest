using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject overlay;

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown windowModeDropdown;
    public TMP_Dropdown qualityDropdown;
    public Slider volumeSlider;

    Resolution[] resolutions;
    RefreshRate refreshRate;

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

    // Start is called before the first frame update
    void Start()
    {
        refreshRate = Screen.currentResolution.refreshRateRatio;
        resolutions = Screen.resolutions.Where(resolution => resolution.refreshRateRatio.value == refreshRate.value).ToArray();

        int currentResolutionIndex = 0;
        List<string> options = new List<string>();
        int i = 0;
        foreach (Resolution r in resolutions)
        {
            string option = r.width + " x " + r.height;
            options.Add(option);

            if (r.width == PlayerPrefs.GetInt("ScreenWidth", Screen.currentResolution.width) && r.height == PlayerPrefs.GetInt("ScreenHeight", Screen.currentResolution.height))
                currentResolutionIndex = i;

            i++;
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.Windowed:
                windowModeDropdown.value = 0;
                break;
            case FullScreenMode.ExclusiveFullScreen:
                windowModeDropdown.value = 1;
                break;
            case FullScreenMode.FullScreenWindow:
                windowModeDropdown.value = 2;
                break;
            default:
                break;
        }
        windowModeDropdown.RefreshShownValue();

        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.75f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetWindowMode(int windowModeIndex)
    {


        PlayerPrefs.SetInt("WindowMode", windowModeIndex);
        switch (windowModeIndex)
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
        PlayerPrefs.Save();

        //ForceReset();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution r = resolutions[resolutionIndex];
        PlayerPrefs.SetInt("ScreenWidth", r.width);
        PlayerPrefs.SetInt("ScreenHeight", r.height);
        Screen.SetResolution(r.width, r.height, Screen.fullScreenMode);
        PlayerPrefs.Save();

        //ForceReset();
    }

    public void SetQuality(int qualityIndex)
    {


        PlayerPrefs.SetInt("Quality", qualityIndex);
        Debug.Log("Changing Quality to " + qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.Save();

        //ForceReset();
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("Volume", volume);
        float dB = AudioManager.LinearToDecibel(volume);
        audioMixer.SetFloat("masterVolume", dB);
        PlayerPrefs.Save();
    }

    public void ForceReset()
    {
        StartCoroutine(ResetOverlay());
    }

    private IEnumerator ResetOverlay()
    {
        // Optionally clear any current selection
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        // Disable the overlay
        FindObjectOfType<PauseMenu>().optionsMenuUI.SetActive(false);

        // Wait one frame (you can increase the delay if needed)
        yield return null;

        // Re-enable the overlay
        FindObjectOfType<PauseMenu>().optionsMenuUI.SetActive(false);

        // Clear selection again after reactivation if needed
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

}
