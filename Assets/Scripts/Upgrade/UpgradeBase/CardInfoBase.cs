using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CardDataBase",
    menuName = "Cards/Card Database")]
public class CardInfoBase : ScriptableObject
{
    public List<UnitData> allCards = new List<UnitData>();

    public UnitData GetCard(string cardId)
    {
        return allCards.Find(card => card.name == cardId);
    }
}