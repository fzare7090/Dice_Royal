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
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (UnitData unit in cardDatabase.allCards)
        {
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