using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CardCollectionData",
    menuName = "Cards/Card Collection Data")]
public class CardCollectionData : ScriptableObject
{
    public List<CardData> cards = new List<CardData>();

    public CardData GetCard(string cardId)
    {
        return cards.Find(card => card.cardId == cardId);
    }

    public bool HasCard(string cardId)
    {
        return GetCard(cardId) != null;
    }

    public void AddCard(string cardId)
    {
        if (HasCard(cardId))
            return;

        cards.Add(new CardData
        {
            cardId = cardId,
            level = 1,
            ownedCopies = 0
        });
    }
}