using UnityEngine;
using TMPro;

/// --------------------------------------------------------
/// PlayerPoints
///
/// Tracks the player’s total points and updates the UI.
/// Points are used for perks, upgrades, or shop purchases.
/// --------------------------------------------------------
public class PlayerPoints : MonoBehaviour
{
    public int points;                              // Current point total
    public TextMeshProUGUI pointsText;              // UI display for points

    private void Start()
    {
        // --- Initialize UI at game start ---
        UpdatePointsUI();
    }

    /// Adds points and updates the UI
    public void AddPoints(int amount)
    {
        points += amount;
        UpdatePointsUI();
    }

    /// Attempts to spend points. Returns true if successful.
    public bool SpendPoints(int amount)
    {
        if (points >= amount)
        {
            points -= amount;
            UpdatePointsUI();
            return true;
        }

        return false;
    }

    /// Updates the point UI text
    private void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            // Pad the display to keep text alignment consistent
            pointsText.text = $"Points: {points}".PadLeft(8);
        }
    }
}
