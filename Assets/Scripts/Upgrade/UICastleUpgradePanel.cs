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
    [SerializeField] private UIPanelAnimator animator;

    private void Awake()
    {
        animator = GetComponent<UIPanelAnimator>();
    }
    private void Start()
    {
        animator.Hide(); 

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
            $" سطح {info.currentLevel}/{info.maxLevel}\n";

        currentStatsText.text =
            $"سلامتی: {info.currentHP:F0}\n" +
            $"قدرت حمله: {info.currentDamage:F0}";

        if (info.canUpgrade)
        {
            nextStatsText.text =
                $"سلامتی : {info.nextHP:F0}\n" +
                $"قدرت حمله : {info.nextDamage:F0}\n";

            upgradeCostText.text =
                $"هزینه ارتقا: : {info.nextLevelCost} ";
        }
        else
        {
            nextStatsText.text = " بالاترین سطح ";
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
