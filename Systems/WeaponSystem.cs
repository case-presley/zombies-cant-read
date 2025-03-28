using System.Collections;
using UnityEngine;
using TMPro;

/// --------------------------------------------------------
/// WeaponSystem
///
/// Handles weapon behavior for the RPK:
/// - Shooting (raycast-based)
/// - Reloading with delay and animation text
/// - Stat upgrades and effects (fire rate, damage, etc.)
/// - Displays ammo UI
///
/// All values are currently hardcoded for a single weapon.
/// --------------------------------------------------------
public class WeaponSystem : MonoBehaviour
{
    public Camera playerCamera;                        // Camera used for aiming/shooting
    public LayerMask hitMask;                          // What the weapon can hit

    public TMP_Text reloadText;                        // UI element for reloading feedback
    public GameObject reloadPanel;                     // UI panel shown while reloading
    public TextMeshProUGUI ammoText;                   // UI element for ammo count
    public GameObject bloodEffectPrefab;               // Particle effect for hits

    public Transform weaponHolder;                     // Where the weapon sits
    public GameObject rpkModel;                        // RPK model to activate

    public PlayerStats playerStats;                    // Stats that affect weapon behavior
    public PlayerPoints playerPoints;                  // Used for ammo refill costs

    // --- RPK STATS ---
    private const string gunName = "RPK";
    private int magazineSize = 40;
    private int maxReserveAmmo = 200;
    private float fireRate = 0.1f;
    private float reloadTime = 3.5f;
    private float damage = 30f;

    private int currentAmmo;
    private int reserveAmmo;
    private bool isReloading;
    private float nextFireTime;

    private void Start()
    {
        // --- Initialize ammo and visuals ---
        currentAmmo = magazineSize;
        reserveAmmo = maxReserveAmmo;
        EquipRPK();
    }

    private void Update()
    {
        // --- Sync weapon direction with camera ---
        weaponHolder.rotation = playerCamera.transform.rotation;

        // --- Update ammo UI ---
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {reserveAmmo}".PadLeft(8);
        }

        // --- Handle firing ---
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && !isReloading)
        {
            if (currentAmmo > 0)
            {
                Shoot();
            }
            else
            {
                StartCoroutine(Reload());
            }
        }

        // --- Manual reload input ---
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    /// Fires a raycast shot and applies damage/effects
    private void Shoot()
    {
        AudioManager.instance.PlayGunshot();

        nextFireTime = Time.time + fireRate / playerStats.GetFireRateMultiplier();
        currentAmmo--;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 100f, hitMask))
        {
            if (hit.collider.CompareTag("Zombie"))
            {
                int modifiedDamage = Mathf.RoundToInt(damage * playerStats.GetDamageMultiplier());
                hit.collider.GetComponent<ZombieHealth>().TakeDamage(modifiedDamage);

                // Spawn blood hit effect
                GameObject bloodEffect = Instantiate(bloodEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodEffect, 0.5f);
            }
            else if (hit.collider.CompareTag("Bookshelf"))
            {
                Debug.Log("Bullet hit an obstacle and stopped.");
                return;
            }
        }

        Debug.Log($"{gunName} fired! Ammo: {currentAmmo} / {reserveAmmo}");
    }

    /// Reloads the weapon over time with UI feedback
    private IEnumerator Reload()
    {
        if (reserveAmmo <= 0) yield break;

        isReloading = true;
        reloadPanel.SetActive(true);

        float modifiedReloadTime = reloadTime * playerStats.GetReloadMultiplier();

        for (float timer = modifiedReloadTime; timer > 0; timer -= 0.1f)
        {
            reloadText.text = $"Reloading... {timer:F1}s";
            yield return new WaitForSeconds(0.1f);
        }

        int ammoToReload = Mathf.Min(magazineSize, reserveAmmo);
        currentAmmo = ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        reloadPanel.SetActive(false);
    }

    /// Activates the RPK model
    private void EquipRPK()
    {
        rpkModel.SetActive(true);
        Debug.Log($"Equipped {gunName}");
    }

    /// Refills reserve ammo if the player has enough points
    public void RefillAmmo()
    {
        if (playerPoints.points >= 500)
        {
            reserveAmmo = maxReserveAmmo;
            playerPoints.SpendPoints(500);
        }
        else
        {
            Debug.Log("Not enough points!");
        }
    }

    /// Applies stat boosts to the RPK
    public void UpgradeRPK()
    {
        Debug.Log("Upgrading RPK...");

        damage *= 1.5f;
        currentAmmo += 10;
        reserveAmmo += 50;
        fireRate *= 0.8f;

        Debug.Log($"RPK Upgraded: Damage = {damage}, Mag = {currentAmmo}, Reserve = {reserveAmmo}, Fire Rate = {fireRate}");
    }
}
