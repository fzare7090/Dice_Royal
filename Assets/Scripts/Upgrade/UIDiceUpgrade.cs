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
    [SerializeField] private UIPanelAnimator animator;

    private void Awake()
    {
        animator = GetComponent<UIPanelAnimator>();
    }
    private void Start()
    {
        animator.Hide();

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
            $"ضریب بعدی : {info.currentLevel}/{info.maxLevel}";

        currentStatsText.text =
            $"ضریب فعلی : x{info.currentMultiplier}";

        if (info.canUpgrade)
        {
            nextStatsText.text =
                $"ضریب بعدی: x{info.nextMultiplier}";

            upgradeCostText.text =
                $"هزینه ارتقا: {info.nextLevelCost} سکه";
        }
        else
        {
            nextStatsText.text = "بالاترین سطح";
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
    public void OpenPanel()
    {
        animator.Show();
        UpdateUI();
    }

    public void ClosePanel()
    {
        animator.Hide();
    }
}
