using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Libraries & Sources")]
    [SerializeField] private SoundLibrary sfxLibrary;
    [SerializeField] private AudioSource sfx2DSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixer mainMixer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Assign mixer groups to the audio sources automatically
            ApplyMixerGroups();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to Unity's scene loading callback
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Start()
    {
        ApplySavedVolumes();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SoundManager] Scene Loaded: {scene.name}");
        ApplySavedVolumes();

        if (scene.name == "Menu")
        {
            Debug.Log("[SoundManager] Scene is 'Menu' -> Attempting to play MainTheme...");
            PlayMusic("MainTheme");
        }
    }

    /// <summary>
    /// Fetches saved volume levels from PlayerPrefs and updates the AudioMixer parameters.
    /// </summary>
    public void ApplySavedVolumes()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        if (mainMixer != null)
        {
            mainMixer.SetFloat("MusicVol", Mathf.Log10(Mathf.Max(music, 0.0001f)) * 20f);
            mainMixer.SetFloat("SFXVol", Mathf.Log10(Mathf.Max(sfx, 0.0001f)) * 20f);
        }
    }

    private void ApplyMixerGroups()
    {
        if (sfx2DSource != null && sfxMixerGroup != null)
        {
            sfx2DSource.outputAudioMixerGroup = sfxMixerGroup;
        }

        if (musicSource != null && musicMixerGroup != null)
        {
            musicSource.outputAudioMixerGroup = musicMixerGroup;
        }
    }

    // ==========================================
    // SFX METHODS
    // ==========================================

    /// <summary>
    /// Plays a direct AudioClip using the 2D SFX source.
    /// </summary>
    public void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfx2DSource == null) return;

        sfx2DSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Plays a 2D SFX lookup by string name from the SoundLibrary.
    /// </summary>
    public void PlaySound2D(string soundName)
    {
        if (sfxLibrary == null || sfx2DSource == null) return;

        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
        if (clip != null)
        {
            Debug.Log($"[SoundManager] Playing 2D SFX: {soundName}");
            PlaySound2D(clip);
        }
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos);
        }
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        if (sfxLibrary == null) return;

        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
        PlaySound3D(clip, pos);
    }

    // ==========================================
    // MUSIC METHODS
    // ==========================================

    /// <summary>
    /// Plays background music using a name from your SoundLibrary.
    /// </summary>
    public void PlayMusic(string soundName, bool loop = true)
    {
        if (sfxLibrary == null)
        {
            Debug.LogError("[SoundManager] CRITICAL: sfxLibrary is NULL!");
            return;
        }

        AudioClip clip = sfxLibrary.GetClipFromName(soundName);

        if (clip == null)
        {
            Debug.LogError($"[SoundManager] CRITICAL: Could not find AudioClip with name '{soundName}' in SoundLibrary!");
            return;
        }

        Debug.Log($"[SoundManager] Found clip '{clip.name}'. Assigning to musicSource...");
        PlayMusic(clip, loop);
    }

    /// <summary>
    /// Plays background music from a direct AudioClip.
    /// </summary>
    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (musicSource == null)
        {
            Debug.LogError("[SoundManager] CRITICAL: musicSource (AudioSource) is NULL!");
            return;
        }

        if (musicClip == null)
        {
            Debug.LogError("[SoundManager] CRITICAL: musicClip passed to PlayMusic is NULL!");
            return;
        }

        if (musicSource.isPlaying && musicSource.clip == musicClip)
        {
            Debug.Log("[SoundManager] Music is already playing this exact clip. Skipping restart.");
            return;
        }

        musicSource.clip = musicClip;
        musicSource.loop = loop;
        musicSource.Play();
        Debug.Log($"[SoundManager] SUCCESS: Playing '{musicClip.name}' on {musicSource.name}! isPlaying = {musicSource.isPlaying}");
    }

    /// <summary>
    /// Stops the currently playing background music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}