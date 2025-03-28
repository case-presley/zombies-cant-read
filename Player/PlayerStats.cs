using System.Collections;
using UnityEngine;

/// --------------------------------------------------------
/// PlayerStats
///
/// Core stats and upgrades for the player:
/// - Tracks current and max health
/// - Handles damage and temporary health regeneration
/// - Stores multipliers for movement, reload, fire rate, and damage
/// - Supports perks like increased speed or unlocking third weapon
/// Used by weapons, UI, and game systems.
/// --------------------------------------------------------
public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;                  // Maximum player health
    public int currentHealth = 100;              // Current player health

    public float movementSpeed = 5f;             // Base movement speed
    public float reloadMultiplier = 1f;          // Reload speed modifier
    public float fireRateMultiplier = 1f;        // Fire rate modifier
    public float damageMultiplier = 1f;          // Weapon damage modifier
    public int maxWeapons = 2;                   // Number of weapons player can hold

    public GameOverManager gameOverManager;      // Reference to trigger game over

    /// Applies incoming damage and checks for death
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");
            gameOverManager.ShowGameOverScreen();
        }

        // Trigger delayed regeneration (temporary system)
        StartCoroutine(RegenerateHealth());
    }

    /// Regenerates full health after a short delay
    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(3f);
        currentHealth = maxHealth;
    }

    // --- PERK UPGRADE FUNCTIONS ---

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth = amount;
    }

    public void IncreaseSpeed(float multiplier)
    {
        movementSpeed *= multiplier;
    }

    public void ReduceReloadTime(float multiplier)
    {
        reloadMultiplier *= multiplier;
    }

    public void UnlockThirdWeaponSlot()
    {
        maxWeapons = 3;
    }

    public void IncreaseFireRate(float multiplier)
    {
        fireRateMultiplier *= multiplier;
    }

    public void IncreaseDamage(float multiplier)
    {
        damageMultiplier *= multiplier;
    }

    // --- STAT ACCESSORS FOR OTHER SYSTEMS ---

    public float GetMovementSpeed()
    {
        return movementSpeed;
    }

    public float GetReloadMultiplier()
    {
        return reloadMultiplier;
    }

    public float GetFireRateMultiplier()
    {
        return fireRateMultiplier;
    }

    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }
}
