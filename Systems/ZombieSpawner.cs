using System.Collections.Generic;
using UnityEngine;

/// --------------------------------------------------------
/// ZombieSpawner
///
/// Spawns zombies at random locations *not* visible to the player:
/// - Uses a list of predefined spawn points
/// - Avoids spawning zombies in the player’s forward view
/// - Instantiates from a hidden inactive prefab
///
/// Attach to a scene manager or spawner object.
/// --------------------------------------------------------
public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;                     // (Unused) original prefab (use hiddenZombie instead)
    public GameObject hiddenZombie;                     // Disabled prefab used for spawning
    public Transform player;                            // Player reference for visibility checks
    public List<Transform> spawnPoints;                 // Spawn points scattered in the scene

    private void Start()
    {
        // --- Make sure the reference zombie stays inactive ---
        SetupHiddenZombie();
    }

    /// Disables the hidden reference zombie on scene load
    private void SetupHiddenZombie()
    {
        hiddenZombie.SetActive(false); // Used purely as a spawn template
    }

    /// Spawns a zombie at a valid (non-visible) point
    public void SpawnZombie()
    {
        Transform spawnPoint = GetValidSpawnPoint();

        if (spawnPoint != null)
        {
            GameObject newZombie = Instantiate(hiddenZombie, spawnPoint.position, Quaternion.identity);
            newZombie.SetActive(true);
        }
    }

    /// Returns a spawn point behind or to the side of the player
    private Transform GetValidSpawnPoint()
    {
        List<Transform> validSpawnPoints = new List<Transform>();

        foreach (Transform spawnPoint in spawnPoints)
        {
            Vector3 toSpawn = spawnPoint.position - player.position;

            // Only use spawn points not directly in front of the player
            if (Vector3.Dot(player.forward, toSpawn.normalized) < 0.5f)
            {
                validSpawnPoints.Add(spawnPoint);
            }
        }

        if (validSpawnPoints.Count > 0)
        {
            return validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];
        }

        return null;
    }
}
