using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICastleUpgradePanel : MonoBehaviour
{
    [SerializeField] private Castle targetCastle;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text currentStatsText;
    [SerializeField] private TMP_Text nextStatsText;
    [SerializeField] private TMP_Text upgradeCostText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Slider progressSlider;

    private CastleUpgrade castleUpgrade;

    private void Start()
    {
        if (targetCastle != null)
        {
            castleUpgrade = targetCastle.GetComponent<CastleUpgrade>();

            if (castleUpgrade != null)
            {
                castleUpgrade.OnUpgradeSuccess += OnUpgradeSuccess;
                castleUpgrade.OnUpgradeFailed += OnUpgradeFailed;
            }
        }
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        }

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (castleUpgrade != null)
        {
            castleUpgrade.OnUpgradeSuccess -= OnUpgradeSuccess;
            castleUpgrade.OnUpgradeFailed -= OnUpgradeFailed;
        }
    }

    private void UpdateUI()
    {
        if (castleUpgrade == null) return;

        UpgradeInfo info = castleUpgrade.GetUpgradeInfo();

        levelText.text = $"Level: {info.currentLevel}/{info.maxLevel}";
        currentStatsText.text = $"HP: {info.currentHP:F0}\nDamage: {info.currentDamage:F0}";

        if (info.canUpgrade)
        {
            nextStatsText.text = $"Next HP: {info.nextHP:F0}\nNext Damage: {info.nextDamage:F0}";
            upgradeCostText.text = $"Cost: {info.nextLevelCost}";
        }
        else
        {
            nextStatsText.text = "MAX LEVEL!";
            upgradeCostText.text = "---";
        }

        upgradeButton.interactable = info.canUpgrade;
        progressSlider.value = castleUpgrade.GetUpgradeProgress();
    }

    private void OnUpgradeSuccess(int newLevel)
    {
        UpdateUI();
        Debug.Log($"Castle upgraded to level {newLevel}!");
    }

    private void OnUpgradeFailed(string reason)
    {
        Debug.LogWarning($"Upgrade failed: {reason}");
    }


    private void OnUpgradeButtonClick()
    {
        if (castleUpgrade != null)
        {
            castleUpgrade.TryUpgrade();
        }
    }
}