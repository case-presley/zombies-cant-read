using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// --------------------------------------------------------
/// GameOverManager
///
/// Controls the game over screen and related behavior:
/// - Displays final player stats (books shelved, zombies killed)
/// - Disables movement and unlocks the cursor
/// - Handles restart and main menu navigation
///
/// Trigger `ShowGameOverScreen()` when the player dies
/// --------------------------------------------------------
public class GameOverManager : MonoBehaviour
{
    public GameObject deathScreenPanel;                  // UI panel shown when player dies
    public TMP_Text booksShelvedText;                    // Displays number of books shelved
    public TMP_Text zombiesKilledText;                   // Displays number of zombies killed

    public InteractionSystem interactionSystem;          // Tracks how many books were placed
    public RoundManager roundManager;                    // Tracks total zombie kills

    public Button restartButton;                         // Restart game button
    public Button mainMenuButton;                        // Return to main menu button
    public FirstPersonMovement firstPersonMovement;      // Reference to disable movement on death

    private void Start()
    {
        // --- Initial setup: hide death screen and assign button actions ---
        deathScreenPanel.SetActive(false);

        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    /// Triggers the game over screen and finalizes player state
    public void ShowGameOverScreen()
    {
        // --- Show UI elements ---
        deathScreenPanel.SetActive(true);

        // --- Display final game stats ---
        int booksShelved = interactionSystem.GetBooksShelved();
        int zombiesKilled = roundManager.GetZombiesKilled();

        booksShelvedText.text = $"Books Shelved: {booksShelved}";
        zombiesKilledText.text = $"Zombies Killed: {zombiesKilled}";

        // --- Unlock cursor and disable movement ---
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        firstPersonMovement.enabled = false;

        // --- Freeze game time ---
        Time.timeScale = 0f;
    }

    /// Reloads the current scene and resumes game time
    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// Loads the main menu scene and resumes game time
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
