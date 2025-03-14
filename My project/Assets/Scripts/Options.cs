using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown windowModeDropdown;
    public TMP_Dropdown qualityDropdown;
    public Slider volumeSlider;

    Resolution[] resolutions;
    RefreshRate refreshRate;

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
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution r = resolutions[resolutionIndex];
        PlayerPrefs.SetInt("ScreenWidth", r.width);
        PlayerPrefs.SetInt("ScreenHeight", r.height);
        Screen.SetResolution(r.width, r.height, Screen.fullScreenMode);
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        PlayerPrefs.SetInt("Quality", qualityIndex);
        Debug.Log("Changing Quality to " + qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("Volume", volume);
        float dB = AudioManager.LinearToDecibel(volume);
        audioMixer.SetFloat("masterVolume", dB);
        PlayerPrefs.Save();
    }
}
