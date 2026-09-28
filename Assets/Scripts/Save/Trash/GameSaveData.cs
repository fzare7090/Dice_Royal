using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    //====================
    // Save Info
    //====================

    public int saveVersion = 1;

    public string saveTime;

    //====================
    // Player Resources
    //====================

    public int coin;

    public int gem;

    //====================
    // Progress
    //====================

    public int castleLevel = 1;

    public int diceLevel = 1;

    //====================
    // Cards
    //====================

    public List<CardSaveInfo> cards = new();

    //====================
    // Constructor
    //====================

    public GameSaveData()
    {
        saveTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    }
}