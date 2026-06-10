using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardDataBase", menuName = "Game/CardDataBase")]
public class CardDataBase : ScriptableObject
{
    public List<UnitData> allCards;
}
