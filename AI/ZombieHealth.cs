using System.Collections;
using UnityEngine;

/// --------------------------------------------------------
/// ZombieHealth
///
/// Handles health tracking and damage for zombies:
/// - Takes damage from weapons
/// - Triggers death via ZombieAI
/// - Notifies RoundManager when killed
///
/// Attach this to all zombie prefabs.
/// --------------------------------------------------------
public class ZombieHealth : MonoBehaviour
{
    public float baseHealth = 100f;                   // Default starting health
    private float currentHealth;

    public ZombieAI zombieAI;                         // Reference to control death sequence
    public RoundManager roundManager;                 // Notified when this zombie dies

    private void Start()
    {
        // --- Set initial health ---
        currentHealth = baseHealth; // No health scaling used
    }

    /// Called when the zombie takes damage from a weapon
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Zombie took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0f)
        {
            // --- Notify round manager ---
            if (roundManager != null)
            {
                roundManager.OnZombieKilled(gameObject);
            }

            // --- Trigger death sequence ---
            zombieAI.Die();

            // Optional coroutine (reserved for pooling, etc.)
            StartCoroutine(ZombieDie(10));
        }
    }

    /// Placeholder for post-death delay (unused, reserved for pooling logic)
    private IEnumerator ZombieDie(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
