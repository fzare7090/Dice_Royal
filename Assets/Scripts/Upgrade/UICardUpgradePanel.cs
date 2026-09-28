using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICardUpgradePanel : MonoBehaviour
{
    [SerializeField] private CardUpgrade cardUpgrade;

    [Header("Current Card")]
    private CardData currentCard;
    private UnitData currentUnit;

    [Header("UI")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text currentStatsText;
    [SerializeField] private TMP_Text nextStatsText;
    [SerializeField] private TMP_Text upgradeCostText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Slider progressSlider;


    private void Start()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        }
    }


    private void OnDestroy()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonClick);
        }
    }


    // این متد را زمانی صدا بزن که کارت انتخاب می‌شود
    public void SetCard(CardData card, UnitData unit)
    {
        currentCard = card;
        currentUnit = unit;

        UpdateUI();
    }


    private void UpdateUI()
    {
        if (cardUpgrade == null ||
            currentCard == null ||
            currentUnit == null)
            return;


        UpgradeCardInfo info =
            cardUpgrade.GetUpgradeInfo(currentCard, currentUnit);


        levelText.text =
            $"سطح {info.currentLevel}/{info.maxLevel}";


        currentStatsText.text =
            $"سلامتی: {info.currentHP:F0}\n" +
            $"قدرت حمله: {info.currentDamage:F0}\n" +
            $"سرعت: {info.currentAttackSpeed:F2}";


        if (info.canUpgrade)
        {
            nextStatsText.text =
                $"سلامتی: {info.nextHP:F0}\n" +
                $"قدرت حمله: {info.nextDamage:F0}\n" +
                $"سرعت: {info.nextAttackSpeed:F2}";


            upgradeCostText.text =
                $"هزینه ارتقا: {info.nextLevelCost}\n" +
                $"کارت لازم: {info.cardsNeeded}";
        }
        else
        {
            nextStatsText.text = "بالاترین سطح";
            upgradeCostText.text = "---";
        }


        upgradeButton.interactable = info.canUpgrade;


        progressSlider.value =
            cardUpgrade.GetUpgradeProgress(currentCard);
    }



    private void OnUpgradeButtonClick()
    {
        if (cardUpgrade == null ||
            currentCard == null)
            return;


        bool success =
            cardUpgrade.TryUpgrade(currentCard);


        if (success)
        {
            UpdateUI();
        }
    }
}