using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Control Guide Window")]
    [SerializeField] private GameObject controlGuidePanel;

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Start()
    {
        // Set slider range from 0.0001 to 1 (prevents log10 math errors with 0)
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

        // Load saved values or set defaults to 0.75f
        float savedMusic = PlayerPrefs.GetFloat(MUSIC_KEY, 0.75f);
        float savedSFX = PlayerPrefs.GetFloat(SFX_KEY, 0.75f);

        // Apply saved values to UI and Audio Mixer
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        // Add UI Listeners dynamically
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        // AudioMixer volume works on a logarithmic scale (-80dB to 20dB)
        float dB = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("MusicVol", dB);

        // Save setting
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("SFXVol", dB);

        // Save setting
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
    }
}