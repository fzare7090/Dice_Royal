using UnityEngine;

public static class CardCollectionExtensions
{
    public static CardData GetOrCreateCard(
        this CardCollectionData collection,
        string cardId)
    {
        CardData card = collection.GetCard(cardId);

        if (card == null)
        {
            card = new CardData
            {
                cardId = cardId,
                level = 1,
                ownedCopies = 0
            };

            collection.cards.Add(card);
        }

        return card;
    }
}