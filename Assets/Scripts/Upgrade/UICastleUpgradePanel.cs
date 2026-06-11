using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;


public class UICastleUpgradePanel : MonoBehaviour
{
    [SerializeField] private CastleUpgrade castleUpgrade;

    [Header("UI")]
    [SerializeField] private RTLTextMeshPro levelText;
    [SerializeField] private RTLTextMeshPro currentStatsText;
    [SerializeField] private RTLTextMeshPro nextStatsText;
    [SerializeField] private RTLTextMeshPro upgradeCostText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Slider progressSlider;

    private void Start()
    {
        if (castleUpgrade != null)
        {
            castleUpgrade.OnUpgradeSuccess += OnUpgradeSuccess;
            castleUpgrade.OnUpgradeFailed += OnUpgradeFailed;
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
        if (castleUpgrade == null)
            return;

        UpgradeInfo info = castleUpgrade.GetUpgradeInfo();

        levelText.text =
            $"Level {info.currentLevel}/{info.maxLevel}";

        currentStatsText.text =
            $"HP: {info.currentHP:F0}\n" +
            $"Damage: {info.currentDamage:F0}";

        if (info.canUpgrade)
        {
            nextStatsText.text =
                $"Next HP: {info.nextHP:F0}\n" +
                $"Next Damage: {info.nextDamage:F0}";

            upgradeCostText.text =
                $"Cost: {info.nextLevelCost} Coins";
        }
        else
        {
            nextStatsText.text = "MAX LEVEL";
            upgradeCostText.text = "---";
        }

        upgradeButton.interactable = info.canUpgrade;
        progressSlider.value = castleUpgrade.GetUpgradeProgress();
    }

    private void OnUpgradeButtonClick()
    {
        castleUpgrade?.TryUpgrade();
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
