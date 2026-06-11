using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;

public class UIDiceUpgradePanel : MonoBehaviour
{
    [SerializeField] private DiceUpgrade targetDiceUpgrade;

    [Header("UI")]
    [SerializeField] private RTLTextMeshPro levelText;
    [SerializeField] private RTLTextMeshPro currentStatsText;
    [SerializeField] private RTLTextMeshPro nextStatsText;
    [SerializeField] private RTLTextMeshPro upgradeCostText;
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
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        }

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
        if (targetDiceUpgrade == null)
            return;

        DiceUpgradeInfo info = targetDiceUpgrade.GetUpgradeInfo();

        levelText.text =
            $"Level {info.currentLevel}/{info.maxLevel}";

        currentStatsText.text =
            $"Current Multiplier: x{info.currentMultiplier}";

        if (info.canUpgrade)
        {
            nextStatsText.text =
                $"Next Multiplier: x{info.nextMultiplier}";

            upgradeCostText.text =
                $"Cost: {info.nextLevelCost} Coins";
        }
        else
        {
            nextStatsText.text = "MAX LEVEL";
            upgradeCostText.text = "---";
        }

        upgradeButton.interactable = info.canUpgrade;
        progressSlider.value = targetDiceUpgrade.GetUpgradeProgress();
    }

    private void OnUpgradeButtonClick()
    {
        targetDiceUpgrade?.TryUpgrade();
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
