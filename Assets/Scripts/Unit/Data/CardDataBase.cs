using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "CardDataBase", menuName = "Game/CardDataBase")]
public class CardDataBase : ScriptableObject
{   
    public List<UnitData> allCards;
}
