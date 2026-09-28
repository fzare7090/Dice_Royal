using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDetailsPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardUpgradeData upgradeData;
    [SerializeField] private CardCollectionData collectionData;
    [SerializeField] private PlayerResourceData playerResource;

    //[SerializeField] private CardUpgrade cardUpgrade;

    [Header("UI")]
    [SerializeField] private RTLTextMeshPro cardNameText;
    [SerializeField] private RTLTextMeshPro levelText;
    [SerializeField] private RTLTextMeshPro currentStatsText;
    [SerializeField] private RTLTextMeshPro nextStatsText;
    [SerializeField] private RTLTextMeshPro costText;
    [SerializeField] private TMP_Text copiesText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private UIPanelAnimator animator;
    [SerializeField] private Image cardImage;

    private UnitData currentUnit;
    private CardData currentCard;
    private void Awake()
    {
        animator = GetComponent<UIPanelAnimator>();
    }
    private void Start()
    {
        animator.Hide();

        upgradeButton.onClick.AddListener(UpgradeCurrentCard);
    }

    public void ShowCard(UnitData unit, CardData cardData)
    {
        animator.Show();

        RectTransform rect =
       GetComponent<RectTransform>();

        rect.anchoredPosition =
            new Vector2(7.3433e-05f, 22.648f);

        currentUnit = unit;
        currentCard = cardData;

        Refresh();
    }
 

    private void Refresh()
    {
        if (currentUnit == null || currentCard == null)
            return;

        int level = currentCard.level;

        bool hasNextLevel =
            level < upgradeData.maxLevel;


        cardNameText.text = currentUnit.name;
        if (cardImage != null)
        {
            cardImage.sprite = currentUnit.cardIcon;
            cardImage.enabled = currentUnit.cardIcon != null;
        }

        levelText.text =
            $" سطح  {level}/{upgradeData.maxLevel}";

        float currentHP =
            currentUnit.hp *
            upgradeData.hpMultiplier[level - 1];

        float currentDamage =
            currentUnit.attackDamage *
            upgradeData.damageMultiplier[level - 1];

        float currentSpeed =
            currentUnit.attackSpeed *
            upgradeData.attackSpeedMultiplier[level - 1];

        currentStatsText.text =
            $"سلامتی: {currentHP:F0} \n" +
            $"قدرت حمله: {currentDamage:F0} \n " +
            $"سرعت: {currentSpeed:F2}";

        if (hasNextLevel)
        {
            int cost = upgradeData.upgradeCost[level - 1];
            int requiredCopies = upgradeData.cardCopiesNeeded[level - 1];

            float nextHP =
                currentUnit.hp *
                upgradeData.hpMultiplier[level];

            float nextDamage =
                currentUnit.attackDamage *
                upgradeData.damageMultiplier[level];

            float nextSpeed =
                currentUnit.attackSpeed *
                upgradeData.attackSpeedMultiplier[level];

            nextStatsText.text =
                $"سلامتی: {currentHP:F0} \n" +
                $"قدرت حمله: {currentDamage:F0} \n" +
                $"سرعت: {nextSpeed:F2}";

            costText.text =
                $"هزینه ارتقا: {cost} سکه";

            copiesText.text =
                $"کارت‌های لازم: {currentCard.ownedCopies}/" +
                $"{requiredCopies}";

            bool hasEnoughCoins =
                playerResource != null && playerResource.GetCoin() >= cost;

            bool hasEnoughCopies =
                currentCard.ownedCopies >= requiredCopies;

            upgradeButton.interactable =
                hasEnoughCoins && hasEnoughCopies;
        }
        else
        {
            nextStatsText.text = " بالاترین سطح ";
            costText.text = "---";
            copiesText.text = "---";
            upgradeButton.interactable = false;
        }
    }

    private void UpgradeCurrentCard()
    {
        if (currentCard == null)
            return;

        if (currentCard.level >= upgradeData.maxLevel)
            return;

        int cost = upgradeData.upgradeCost[currentCard.level - 1];
        int requiredCopies = upgradeData.cardCopiesNeeded[currentCard.level - 1];

        if (playerResource.GetCoin() < cost)
            return;

        if (currentCard.ownedCopies < requiredCopies)
            return;

        playerResource.SpendCoin(cost);

        currentCard.ownedCopies -= requiredCopies;
        currentCard.level++;

        SaveManager.Instance.SaveGame();

        AutoSaveEvents.CardChanged();
        AutoSaveEvents.CoinChanged();

        Refresh();
    }
}
