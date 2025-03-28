using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// --------------------------------------------------------
/// ShopSystem
///
/// Handles the shop UI and gameplay logic for purchasing:
/// - Perks (e.g. Juggernaut, Speed Cola)
/// - Weapon upgrades
///
/// Pauses the game when open, disables player controls,
/// and restores everything upon closing.
/// --------------------------------------------------------
public class ShopSystem : MonoBehaviour
{
    // --- PRICES ---
    public int juggernautCost = 2500;
    public int staminUpCost = 2000;
    public int speedColaCost = 3000;
    public int doubleTapCost = 3500;
    public int upgradeCost = 5000;

    // --- UI REFERENCES ---
    public GameObject shopPanel;
    public GameObject perkPanel;
    public GameObject upgradePanel;

    public Button upgradeButton;
    public Button closeButton;
    public Button juggernautButton;
    public Button staminUpButton;
    public Button speedColaButton;
    public Button doubleTapButton;

    // --- GAME SYSTEM REFERENCES ---
    public PlayerStats playerStats;
    public PlayerPoints playerPoints;
    public WeaponSystem weaponSystem;
    public FirstPersonMovement firstPersonMovement;
    public Canvas canvas; // Main HUD canvas (hidden while in shop)

    private void Start()
    {
        // --- Hide shop UI by default ---
        shopPanel.SetActive(false);
        perkPanel.SetActive(false);
        upgradePanel.SetActive(false);

        // --- Hook up perk buttons ---
        juggernautButton.onClick.AddListener(() => BuyPerk("Juggernaut", juggernautCost));
        staminUpButton.onClick.AddListener(() => BuyPerk("Stamin-Up", staminUpCost));
        speedColaButton.onClick.AddListener(() => BuyPerk("Speed Cola", speedColaCost));
        doubleTapButton.onClick.AddListener(() => BuyPerk("Double Tap", doubleTapCost));

        // --- Hook up other buttons ---
        upgradeButton.onClick.AddListener(UpgradeWeapon);
        closeButton.onClick.AddListener(CloseShop);
    }

    /// Opens the shop and pauses gameplay
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        perkPanel.SetActive(false);
        upgradePanel.SetActive(false);
        canvas.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        firstPersonMovement.enabled = false;
        Time.timeScale = 0f;
    }

    /// Closes the shop and resumes gameplay
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        perkPanel.SetActive(false);
        upgradePanel.SetActive(false);
        canvas.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        firstPersonMovement.enabled = true;
        Time.timeScale = 1f;
    }

    /// Purchases a perk and applies the upgrade to the player
    private void BuyPerk(string perkName, int cost)
    {
        if (!playerPoints.SpendPoints(cost)) return;

        switch (perkName)
        {
            case "Juggernaut":
                playerStats.IncreaseMaxHealth(250);
                break;

            case "Stamin-Up":
                playerStats.IncreaseSpeed(1.2f);
                break;

            case "Speed Cola":
                playerStats.ReduceReloadTime(0.5f);
                break;

            case "Double Tap":
                playerStats.IncreaseFireRate(1.3f);
                playerStats.IncreaseDamage(1.3f);
                break;
        }

        Debug.Log($"You bought {perkName}!");
    }

    /// Upgrades the player's weapon (if enough points)
    private void UpgradeWeapon()
    {
        if (playerPoints.SpendPoints(upgradeCost))
        {
            weaponSystem.UpgradeRPK();
            Debug.Log("Weapon upgraded!");
        }
        else
        {
            Debug.Log("Not enough points!");
        }
    }

    /// Shows the perk selection panel
    public void OpenPerkPanel()
    {
        perkPanel.SetActive(true);
        shopPanel.SetActive(false);
    }

    /// Shows the weapon upgrade panel
    public void OpenUpgradePanel()
    {
        upgradePanel.SetActive(true);
        shopPanel.SetActive(false);
    }
}
