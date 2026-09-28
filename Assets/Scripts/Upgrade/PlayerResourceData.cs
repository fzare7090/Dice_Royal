using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerResourceData", menuName = "Player/Player Resource Data")]
public class PlayerResourceData : ScriptableObject
{
    [Header("Resources")]
    [SerializeField] private int coin;
    [SerializeField] private int gem;

    [Header("Owned Cards")]
    [SerializeField] private List<CardData> cards = new();

    #region Events

    public event Action<int> OnCoinChanged;
    public event Action<int> OnGemChanged;
    public event Action<CardData> OnCardUpdated;

    #endregion

    #region Properties

    public int Coin => coin;
    public int Gem => gem;

    public IReadOnlyList<CardData> Cards => cards;

    #endregion

    #region Coin
    public void SetCoin(int value)
    {
        coin = value;
        AutoSaveEvents.CoinChanged();
    }

    public int GetCoin()
    {
        return coin;
    }

    public void AddCoin(int value)
    {
        coin += value;
        AutoSaveEvents.CoinChanged();
    }

    public int GetGem()
    {
        return gem;
    }
    public void SetGem(int value)
    {
        gem = value;
        AutoSaveEvents.GemChanged();
    }

    public void AddGem(int value)
    {
        gem += value;
        AutoSaveEvents.GemChanged();
    }
    public bool SpendGem(int amount)
    {
        if (amount <= 0)
            return false;

        if (gem < amount)
            return false;

        gem -= amount;
        OnGemChanged?.Invoke(gem);
        return true;
    }

    public bool SpendCoin(int amount)
    {
        if (amount <= 0)
            return false;

        if (coin < amount)
            return false;

        coin -= amount;
        OnCoinChanged?.Invoke(coin);
        return true;
    }

    #endregion

    #region Cards

    public CardData GetOrCreateCard(string cardId)
    {
        CardData card = cards.Find(c => c.cardId == cardId);

        if (card == null)
        {
            card = new CardData
            {
                cardId = cardId,
                level = 1,
                ownedCopies = 0
            };

            cards.Add(card);
            OnCardUpdated?.Invoke(card);
        }

        return card;
    }

    public CardData GetCard(string cardId)
    {
        return cards.Find(c => c.cardId == cardId);
    }

    public void AddCardCopies(string cardId, int amount)
    {
        if (amount <= 0)
            return;

        CardData card = GetOrCreateCard(cardId);

        card.ownedCopies += amount;

        OnCardUpdated?.Invoke(card);
    }

    public bool UpgradeCard(string cardId, int requiredCopies)
    {
        CardData card = GetCard(cardId);

        if (card == null)
            return false;

        if (card.ownedCopies < requiredCopies)
            return false;

        card.ownedCopies -= requiredCopies;
        card.level++;

        OnCardUpdated?.Invoke(card);

        return true;
    }

    public void ClearCards()
    {
        cards.Clear();
    }

    public void AddLoadedCard(CardData card)
    {
        cards.Add(card);
    }

    #endregion
}