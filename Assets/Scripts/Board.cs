using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private KingCastleConfig kingCastle;
    [SerializeField] private List<PrinceCastleConfig> princeCastles = new List<PrinceCastleConfig>();

    private Vector3 boardCenter;
    private Vector3 boardSize; // vctor3?

    private Bounds boardBounds;


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

    public Vector3 GetBoardCenter()
    {
        return boardCenter;
    }
    public Vector3 GetBoardSize()
    {
        return boardSize;
    }
    public Bounds GetBoardBounds()
    {
        return boardBounds;
    }
    public void SetBoardCenter(Vector3 bCentar)
    {
        boardCenter = bCentar;
    }
    public void SetBoardSize(Vector3 bSize)
    {
        boardSize = bSize;
    }
    public void SetBoardBoundes(Bounds bBounds)
    {
        boardBounds = bBounds;
    }


}