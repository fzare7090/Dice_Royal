using UnityEngine;

public class MatchData 
{
    public string matchId;
    public string matchDate;
    public MatchResult result;
    public int trophiesBefore;
    public int trophiesAfter;
    public int trophiesChanged;
    public int playerScore;
    public int opponentScore;
    public float matchDuration;
    public string winnerName;
    public int playerLevel;
    public int opponentLevel;

}

public class GameStatsData
{
    public int totalMatches;
    public int totalWins;
    public int totalLosses;
    public int totalDraws;
    public float winRate;
}

