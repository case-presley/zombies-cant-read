using UnityEngine;

/// --------------------------------------------------------
/// AudioManager
///
/// Handles background music playback and one-shot sound effects
/// (like gunshots). Implemented as a Singleton for global access.
///
/// Usage:
/// - Call `AudioManager.instance.PlayGunshot()` to trigger a gunshot
/// - Automatically plays a random music track on Start
/// --------------------------------------------------------
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;      // Global reference to AudioManager

    public AudioSource musicSource;           // Audio source used for looping background music
    public AudioClip[] backgroundTracks;      // Array of background music tracks

    public AudioSource sfxSource;             // Audio source for playing sound effects
    public AudioClip gunshotSound;            // Preloaded gunshot sound effect

    private void Awake()
    {
        // --- Set up singleton instance ---
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate AudioManagers
        }
    }

    private void Start()
    {
        // --- Start playing a random music track ---
        PlayRandomMusic();
    }

    /// Plays a randomly selected music track from the list
    private void PlayRandomMusic()
    {
        // --- Ensure we have at least one track to play ---
        if (backgroundTracks.Length > 0)
        {
            int index = Random.Range(0, backgroundTracks.Length);

            musicSource.clip = backgroundTracks[index];
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    /// Plays a one-shot sound effect from the given clip
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    /// Plays the default gunshot sound effect
    public void PlayGunshot()
    {
        PlaySFX(gunshotSound);
    }
}
