using UnityEngine;
using UnityEngine.Audio;

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
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        // Apply saved PlayerPrefs volumes as soon as SoundManager initializes
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        if (mainMixer != null)
        {
            mainMixer.SetFloat("MusicVol", Mathf.Log10(Mathf.Max(music, 0.0001f)) * 20f);
            mainMixer.SetFloat("SFXVol", Mathf.Log10(Mathf.Max(sfx, 0.0001f)) * 20f);
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMusic("MainTheme");
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

    public void PlaySound2D(string soundName)
    {
        if (sfxLibrary == null || sfx2DSource == null) return;

        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
        if (clip != null)
        {
            sfx2DSource.PlayOneShot(clip);
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
    /// Plays background music from a direct AudioClip.
    /// </summary>
    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (musicSource == null || musicClip == null) return;

        // Don't restart if the same song is already playing
        if (musicSource.isPlaying && musicSource.clip == musicClip) return;

        musicSource.clip = musicClip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    /// <summary>
    /// Plays background music using a name from your SoundLibrary.
    /// </summary>
    public void PlayMusic(string soundName, bool loop = true)
    {
        if (sfxLibrary == null) return;

        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
        PlayMusic(clip, loop);
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