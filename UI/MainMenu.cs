using UnityEngine;
using UnityEngine.SceneManagement;

/// --------------------------------------------------------
/// MainMenu
///
/// Handles the main menu buttons:
/// - Starts the main game scene
/// - Quits the application
///
/// Hook these methods up to UI button events in the menu
/// --------------------------------------------------------
public class MainMenu : MonoBehaviour
{
    /// Starts the main gameplay scene
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    /// Quits the application (only works in builds)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit game pressed.");
    }
}
