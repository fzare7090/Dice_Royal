using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDetailsPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardUpgradeData upgradeData;
    [SerializeField] private CardCollectionData collectionData;
    [SerializeField] private PlayerResourceData playerResource;

    [Header("UI")]
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text currentStatsText;
    [SerializeField] private TMP_Text nextStatsText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text copiesText;

    [SerializeField] private Button upgradeButton;

    private UnitData currentUnit;
    private CardData currentCard;

    private void Start()
    {
        gameObject.SetActive(false);

        upgradeButton.onClick.AddListener(UpgradeCurrentCard);
    }

    public void ShowCard(UnitData unit, CardData cardData)
    {
        gameObject.SetActive(true);

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

        bool canUpgrade =
            level < upgradeData.maxLevel;

        cardNameText.text = currentUnit.name;

        levelText.text =
            $"Level {level}/{upgradeData.maxLevel}";

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
            $"HP: {currentHP:F0}\n" +
            $"Damage: {currentDamage:F0}\n" +
            $"Speed: {currentSpeed:F2}";

        if (canUpgrade)
        {
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
                $"HP: {nextHP:F0}\n" +
                $"Damage: {nextDamage:F0}\n" +
                $"Speed: {nextSpeed:F2}";

            costText.text =
                $"Cost: {upgradeData.upgradeCost[level - 1]}";

            copiesText.text =
                $"Cards: {currentCard.ownedCopies}/" +
                $"{upgradeData.cardCopiesNeeded[level - 1]}";
        }
        else
        {
            nextStatsText.text = "MAX LEVEL";
            costText.text = "---";
            copiesText.text = "---";
        }

        upgradeButton.interactable = canUpgrade;
    }

    private void UpgradeCurrentCard()
    {
        if (currentCard == null)
            return;

        int level = currentCard.level;

        if (level >= upgradeData.maxLevel)
            return;

        int cost =
            upgradeData.upgradeCost[level - 1];

        int requiredCopies =
            upgradeData.cardCopiesNeeded[level - 1];

        if (playerResource.coin<cost)
            return;

        if (currentCard.ownedCopies < requiredCopies)
            return;

        currentCard.ownedCopies -= requiredCopies;
        currentCard.level++;

        Refresh();
    }
}