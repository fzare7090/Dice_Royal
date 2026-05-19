using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private KingCastleConfig kingCastle;
    [SerializeField] private List<PrinceCastleConfig> princeCastles = new List<PrinceCastleConfig>();

    public KingCastleConfig GetKingCastle()
    {
        return kingCastle;
    }

    public List<PrinceCastleConfig> GetPrinceCastles()
    {
        return princeCastles;
    }


    public void SetKingCastleConfig(KingCastleConfig newKingCastle)
    {
        kingCastle = newKingCastle;
    }

    public void SetPrinceCastleConfig(List<PrinceCastleConfig> newPrinceCastles)
    {
        princeCastles = newPrinceCastles;
    }
    public int GetPrinceCastleCount()
    {
        return princeCastles.Count;

    }



}
