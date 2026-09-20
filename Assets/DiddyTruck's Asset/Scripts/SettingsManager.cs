using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Control Guide UI")]
    [SerializeField] private Toggle controlGuideToggle;
    [SerializeField] private GameObject controlGuidePanel;

    [Header("UI Panels to Toggle")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject objectivePanel;

    private const string CONTROL_GUIDE_KEY = "ControlGuideState";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Start()
    {
        // 1. Configure slider min/max bounds FIRST
        ConfigureSliders();

        // 2. Load saved settings from PlayerPrefs
        LoadSettings();

        // 3. Add UI Listeners AFTER setting initial values
        RegisterListeners();
    }

    private void ConfigureSliders()
    {
        if (musicSlider != null)
        {
            musicSlider.minValue = 0.0001f;
            musicSlider.maxValue = 1f;
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0.0001f;
            sfxSlider.maxValue = 1f;
        }
    }

    private void LoadSettings()
    {
        // --- Volume Settings ---
        float savedMusic = PlayerPrefs.GetFloat(MUSIC_KEY, 0.75f);
        float savedSFX = PlayerPrefs.GetFloat(SFX_KEY, 0.75f);

        // Update UI Sliders
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        // Apply to AudioMixer
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        // --- Control Guide Toggle Settings ---
        bool savedControlGuideState = PlayerPrefs.GetInt(CONTROL_GUIDE_KEY, 0) == 1;

        if (controlGuideToggle != null)
        {
            controlGuideToggle.SetIsOnWithoutNotify(savedControlGuideState);
        }

        if (controlGuidePanel != null)
        {
            controlGuidePanel.SetActive(savedControlGuideState);
        }
    }

    private void RegisterListeners()
    {
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void OnDestroy()
    {
        // Clean up listeners when switching scenes to avoid memory leaks
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }

    // --- Audio Volume Setters ---
    public void SetMusicVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MusicVol", dB);
        }

        PlayerPrefs.SetFloat(MUSIC_KEY, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SFXVol", dB);
        }

        PlayerPrefs.SetFloat(SFX_KEY, value);
        PlayerPrefs.Save();
    }

    // --- Control Guide Toggle ---
    public void ToggleControlGuide(bool isOn)
    {
        if (controlGuidePanel != null)
        {
            controlGuidePanel.SetActive(isOn);
        }

        PlayerPrefs.SetInt(CONTROL_GUIDE_KEY, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSettingsMenu(bool isSettingsOpen)
    {
        // Settings turns ON when isSettingsOpen is true
        if (settingsPanel != null)
            settingsPanel.SetActive(isSettingsOpen);

        // HUD and Objective turn OFF when isSettingsOpen is true (inverted logic)
        if (hudPanel != null)
            hudPanel.SetActive(!isSettingsOpen);

        if (objectivePanel != null)
            objectivePanel.SetActive(!isSettingsOpen);
    }
}