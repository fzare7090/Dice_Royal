using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICardUpgradePanel : MonoBehaviour
{
    [SerializeField] private CardUpgrade targetCardUpgrade;

    [Header("UI")]
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
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        }

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
        if (targetCardUpgrade == null)
            return;

        UpgradeCardInfo info = targetCardUpgrade.GetUpgradeInfo();

        levelText.text =
            $"Level {info.currentLevel}/{info.maxLevel}";

        currentStatsText.text =
            $"HP: {info.currentHP:F0}\n" +
            $"Damage: {info.currentDamage:F0}\n" +
            $"Speed: {info.currentAttackSpeed:F2}";

        if (info.canUpgrade)
        {
            nextStatsText.text =
                $"Next HP: {info.nextHP:F0}\n" +
                $"Next Damage: {info.nextDamage:F0}\n" +
                $"Next Speed: {info.nextAttackSpeed:F2}";

            upgradeCostText.text =
                $"Cost: {info.nextLevelCost}\n" +
                $"Cards Needed: {info.cardsNeeded}";
        }
        else
        {
            nextStatsText.text = "MAX LEVEL";
            upgradeCostText.text = "---";
        }

        upgradeButton.interactable = info.canUpgrade;
        progressSlider.value = targetCardUpgrade.GetUpgradeProgress();
    }

    private void OnUpgradeButtonClick()
    {
        targetCardUpgrade?.TryUpgrade();
    }

    private void OnUpgradeSuccess(int level)
    {
        UpdateUI();
    }

    private void OnUpgradeFailed(string reason)
    {
        Debug.LogWarning(reason);
    }
}