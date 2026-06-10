using UnityEngine;

public class CardUpgradeScrollView : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CardDataBase cardDatabase;
    [SerializeField] private CardCollectionData collectionData;

    [Header("UI")]
    [SerializeField] private CardDetailsPanel detailsPanel;
    [SerializeField] private CardScrollItemUI cardItemPrefab;
    [SerializeField] private Transform content;


    private void Start()
    {
        GenerateCards();
    }
    private void GenerateCards()
    {
        if (content == null)
        {
            Debug.LogError($"{nameof(CardUpgradeScrollView)} is missing a content reference.", this);
            return;
        }

        if (cardDatabase == null)
        {
            Debug.LogError($"{nameof(CardUpgradeScrollView)} is missing a card database reference.", this);
            return;
        }

        if (cardDatabase.allCards == null)
        {
            Debug.LogError($"{nameof(CardDataBase)} has no card list assigned.", cardDatabase);
            return;
        }

        if (collectionData == null)
        {
            Debug.LogError($"{nameof(CardUpgradeScrollView)} is missing a collection data reference.", this);
            return;
        }

        if (cardItemPrefab == null)
        {
            Debug.LogError($"{nameof(CardUpgradeScrollView)} is missing a card item prefab reference.", this);
            return;
        }

        if (detailsPanel == null)
        {
            Debug.LogError($"{nameof(CardUpgradeScrollView)} is missing a details panel reference.", this);
            return;
        }

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (UnitData unit in cardDatabase.allCards)
        {
            if (unit == null)
                continue;

            CardData cardData =
                collectionData.GetOrCreateCard(unit.name);

            CardScrollItemUI item =
                Instantiate(cardItemPrefab, content);

            item.Initialize(
                unit,
                cardData,
                detailsPanel);
        }
    }
}
