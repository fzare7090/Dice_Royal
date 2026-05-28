using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICardUpgradePanel : MonoBehaviour
{
    [SerializeField] private CardUpgrade targetCardUpgrade;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text currentStatsText;
    [SerializeField] private TMP_Text nextStatsText;
    [SerializeField] private TMP_Text upgradeCostText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Slider progressSlider;

    private void Start()
    {
        if (targetCardUpgrade != null)
        {
            targetCardUpgrade.OnUpgradeSuccess += OnUpgradeSuccess;
            targetCardUpgrade.OnUpgradeFailed += OnUpgradeFailed;
        }

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(() => targetCardUpgrade?.TryUpgrade());

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (targetCardUpgrade != null)
        {
            targetCardUpgrade.OnUpgradeSuccess -= OnUpgradeSuccess;
            targetCardUpgrade.OnUpgradeFailed -= OnUpgradeFailed;
        }
    }

    private void UpdateUI()
    {
        if (targetCardUpgrade == null) return;

        UpgradeCardInfo info = targetCardUpgrade.GetUpgradeInfo();

        levelText.text = $"Level: {info.currentLevel}/{info.maxLevel}";
        currentStatsText.text = $"HP: {info.currentHP:F0}\nDamage: {info.currentDamage:F0}\nSpeed: {info.currentAttackSpeed:F2}";

        if (info.canUpgrade)
        {
            nextStatsText.text = $"Next HP: {info.nextHP:F0}\nNext Damage: {info.nextDamage:F0}\nNext Speed: {info.nextAttackSpeed:F2}";
            upgradeCostText.text = $"Cost: {info.nextLevelCost}  |  Cards: {info.cardsNeeded}";
        }
        else
        {
            nextStatsText.text = "MAX LEVEL!";
            upgradeCostText.text = "---";
        }

        upgradeButton.interactable = info.canUpgrade;
        progressSlider.value = targetCardUpgrade.GetUpgradeProgress();
    }

    private void OnUpgradeSuccess(int newLevel)
    {
        UpdateUI();
        Debug.Log($"Card upgraded to level {newLevel}!");
    }

    private void OnUpgradeFailed(string reason)
    {
        Debug.LogWarning($"Upgrade failed: {reason}");
    }
}