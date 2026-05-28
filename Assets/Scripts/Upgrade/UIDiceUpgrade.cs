using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDiceUpgradePanel : MonoBehaviour
{
    [SerializeField] private DiceUpgrade targetDiceUpgrade;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text currentStatsText;
    [SerializeField] private TMP_Text nextStatsText;
    [SerializeField] private TMP_Text upgradeCostText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Slider progressSlider;

    private void Start()
    {
        if (targetDiceUpgrade != null)
        {
            targetDiceUpgrade.OnUpgradeSuccess += OnUpgradeSuccess;
            targetDiceUpgrade.OnUpgradeFailed += OnUpgradeFailed;
        }

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(() => targetDiceUpgrade?.TryUpgrade());

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (targetDiceUpgrade != null)
        {
            targetDiceUpgrade.OnUpgradeSuccess -= OnUpgradeSuccess;
            targetDiceUpgrade.OnUpgradeFailed -= OnUpgradeFailed;
        }
    }

    private void UpdateUI()
    {
        if (targetDiceUpgrade == null) return;

        DiceUpgradeInfo info = targetDiceUpgrade.GetUpgradeInfo();

        levelText.text = $"Dice Level: {info.currentLevel}/{info.maxLevel}";
        currentStatsText.text = $"Current Multiplier: x{info.currentMultiplier}";

        if (info.canUpgrade)
        {
            nextStatsText.text = $"Next Multiplier: x{info.nextMultiplier} (+{info.nextMultiplier - info.currentMultiplier})";
            upgradeCostText.text = $"Cost: {info.nextLevelCost} Elexir";
        }
        else
        {
            nextStatsText.text = "MAX LEVEL!";
            upgradeCostText.text = "---";
        }

        upgradeButton.interactable = info.canUpgrade;
        progressSlider.value = targetDiceUpgrade.GetUpgradeProgress();
    }

    private void OnUpgradeSuccess(int newLevel)
    {
        UpdateUI();
        Debug.Log($"Dice upgraded to level {newLevel}! Multiplier: {targetDiceUpgrade.GetCurrentMultiplier()}");
    }

    private void OnUpgradeFailed(string reason)
    {
        Debug.LogWarning($"Dice upgrade failed: {reason}");
    }
}