using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private KingCastleConfig kingCastle;
    [SerializeField] private PrinceCastleConfig rightPriceCastle;
    [SerializeField] private PrinceCastleConfig leftPriceCastle;


    public KingCastleConfig GetKingCastle()
    {
        return kingCastle;
    }


    public PrinceCastleConfig GetRightPrinceCastles()
    {
        return rightPriceCastle;
    }


    public PrinceCastleConfig GetLeftPrinceCastles()
    {
        return leftPriceCastle;
    }


    public void SetKingCastleConfig(KingCastleConfig newKingCastle)
    {
        kingCastle = newKingCastle;
    }

    public void SetPrinceCastleConfig(List<PrinceCastleConfig> newPrinceCastles)
    {
        //princeCastles = newPrinceCastles;
    }
    public int GetPrinceCastleCount()
    {
        int count = 0;

        if (rightPriceCastle != null && rightPriceCastle.exist)
        {
            count++;
        }
        if (leftPriceCastle != null && leftPriceCastle.exist)
        {
            count++;
        }

        return count;

    }


    public int GetCastleCount()
    {
        int castleCount = GetPrinceCastleCount();

        if (kingCastle.GetCastleHP().exist)
        {
            castleCount = GetPrinceCastleCount() + 1;
        }

        return castleCount;
    }



}
