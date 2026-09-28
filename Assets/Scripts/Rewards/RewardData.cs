using System;
using System.Collections.Generic;

[Serializable]
public class RewardData
{
    public int coins;
    public int gems;
   
    public List<CardReward> cards = new();
}