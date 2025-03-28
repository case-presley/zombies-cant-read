using System.Collections;
using UnityEngine;

/// --------------------------------------------------------
/// RoundManager
///
/// Manages the endless zombie spawning loop and tracks kills:
/// - Spawns zombies continuously using a set spawn rate
/// - Tracks number of zombies defeated by the player
/// - Skips dummy prefab used as a spawn template
///
/// Attach this to a manager object in your scene.
/// --------------------------------------------------------
public class RoundManager : MonoBehaviour
{
    public float spawnRate = 10f;                        // Delay between each zombie spawn
    public ZombieSpawner zombieSpawner;                  // Reference to active spawner
    public GameObject hiddenZombie;                      // Reference to base (inactive) zombie prefab

    private int zombiesKilled;                           // Total kill count

    private void Start()
    {
        zombiesKilled = 0;

        // --- Begin the endless spawn loop ---
        StartSpawning();
    }

    /// Starts the spawn coroutine
    private void StartSpawning()
    {
        StartCoroutine(SpawnZombies());
    }

    /// Spawns a zombie every few seconds
    private IEnumerator SpawnZombies()
    {
        while (true)
        {
            zombieSpawner.SpawnZombie();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    /// Called externally when a zombie is killed
    public void OnZombieKilled(GameObject zombie)
    {
        // Ignore the hidden prefab used as a template
        if (zombie == hiddenZombie) return;

        zombiesKilled++;
    }

    /// Returns the total number of zombies killed
    public int GetZombiesKilled()
    {
        return zombiesKilled;
    }
}
