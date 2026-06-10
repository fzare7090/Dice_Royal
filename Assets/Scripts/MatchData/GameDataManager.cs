using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;

public enum MatchResult
{
    Lose = 0,
    Draw = 1,
    Win = 2
}
// اضافه کردن مقدار سختی بر اساس للول ب

[System.Serializable]


public class DataGameManager : MonoBehaviour
{
    public static DataGameManager Instance { get; private set; }

    private const string MATCH_COUNT_KEY = "TotalMatches";
    private const string MATCH_PREFIX = "Match_";

    [SerializeField] private Coin coinManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (coinManager == null)
        {
            coinManager = FindObjectOfType<Coin>();
        }
    }

    public void SaveMatch(MatchData match)
    {
        int matchCount = PlayerPrefs.GetInt(MATCH_COUNT_KEY, 0);

        match.matchId = matchCount.ToString();

        PersianCalendar pc = new PersianCalendar();
        DateTime now = DateTime.Now;

        string shamsiDate =
            $"{pc.GetYear(now)}/{pc.GetMonth(now):00}/{pc.GetDayOfMonth(now):00} {now:HH:mm:ss}";

        match.matchDate = shamsiDate;

        if (match.trophiesChanged == 0)
        {
            match.trophiesChanged = match.trophiesAfter - match.trophiesBefore;
        }

        string jsonData = JsonUtility.ToJson(match);
        string key = MATCH_PREFIX + matchCount;

        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetInt(MATCH_COUNT_KEY, matchCount + 1);
        PlayerPrefs.Save();

        Debug.Log($"Match {matchCount} saved! Winner: {match.winnerName}, Score: {match.playerScore}-{match.opponentScore}");
    }
    public MatchData LoadMatch(int matchIndex)
    {
        string key = MATCH_PREFIX + matchIndex;

        if (PlayerPrefs.HasKey(key))
        {
            string jsonData = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<MatchData>(jsonData);
        }

        Debug.LogWarning($"Match {matchIndex} not found!");
        return null;
    }

    public MatchData[] GetAllMatches()
    {
        int totalMatches = PlayerPrefs.GetInt(MATCH_COUNT_KEY, 0);
        MatchData[] matches = new MatchData[totalMatches];

        for (int i = 0; i < totalMatches; i++)
        {
            matches[i] = LoadMatch(i);
        }

        return matches;
    }

    public MatchData[] GetMatchesByResult(MatchResult result)
    {
        List<MatchData> resultList = new List<MatchData>();
        MatchData[] allMatches = GetAllMatches();

        foreach (MatchData match in allMatches)
        {
            if (match.result == result)
            {
                resultList.Add(match);
            }
        }

        return resultList.ToArray();
    }

    public void SaveMatchFromGameEnd(
    int winnerIndex,
    int player1Score,
    int player2Score,
    float duration,
    int playerLevel,
    int opponentLevel,
    int trophiesChange = 0)
    {
        MatchData match = new MatchData();

        if (winnerIndex == 0)
        {
            match.result = MatchResult.Win;
            match.winnerName = "Player 1";

            match.playerScore = player1Score;
            match.opponentScore = player2Score;
        }
        else
        {
            match.result = MatchResult.Lose;
            match.winnerName = "Player 2";

            match.playerScore = player2Score;
            match.opponentScore = player1Score;
        }

        match.playerLevel = playerLevel;
        match.opponentLevel = opponentLevel;

        match.trophiesBefore = GetCurrentTrophies();
        match.trophiesAfter = match.trophiesBefore + trophiesChange;
        match.trophiesChanged = trophiesChange;

        match.matchDuration = duration;

        SaveMatch(match);

        if (coinManager != null &&
            match.result == MatchResult.Win)
        {
            int coinReward = CalculateCoinReward();
            coinManager.AddCoin(coinReward);
        }
    }

    public GameStatsData GetGameStats()
    {
        GameStatsData stats = new GameStatsData();
        MatchData[] allMatches = GetAllMatches();

        stats.totalMatches = allMatches.Length;

        foreach (MatchData match in allMatches)
        {
            switch (match.result)
            {
                case MatchResult.Win:
                    stats.totalWins++;
                    break;
                case MatchResult.Lose:
                    stats.totalLosses++;
                    break;
                case MatchResult.Draw:
                    stats.totalDraws++;
                    break;
            }
        }

        stats.winRate = stats.totalMatches > 0 ? (float)stats.totalWins / stats.totalMatches * 100f : 0f;

        return stats;
    }

    public void ClearHistory()
    {
        int totalMatches = PlayerPrefs.GetInt(MATCH_COUNT_KEY, 0);

        for (int i = 0; i < totalMatches; i++)
        {
            PlayerPrefs.DeleteKey(MATCH_PREFIX + i);
        }

        PlayerPrefs.DeleteKey(MATCH_COUNT_KEY);
        PlayerPrefs.Save();

        Debug.Log("Match history cleared!");
    }

    private int GetCurrentTrophies()
    {
        return PlayerPrefs.GetInt("PlayerTrophies", 1000);
    }

    public void SetCurrentTrophies(int trophies)
    {
        PlayerPrefs.SetInt("PlayerTrophies", trophies);
        PlayerPrefs.Save();
    }

    private int CalculateCoinReward()
    {
        return UnityEngine.Random.Range(50, 200);
    }

    public void ShowMatchHistory()
    {
        MatchData[] allMatches = GetAllMatches();

        Debug.Log($"\n========== MATCH HISTORY ({allMatches.Length} matches) ==========");

        foreach (MatchData match in allMatches)
        {
            string resultText = match.result == MatchResult.Win ? "WIN" : (match.result == MatchResult.Lose ? "LOSE" : "DRAW");
            Debug.Log($"Match {match.matchId}: {resultText} | {match.playerScore}-{match.opponentScore} | Winner: {match.winnerName} | {match.matchDuration}s | {match.matchDate}");
        }


    }

    public void ShowWinsOnly()
    {
        MatchData[] wins = GetMatchesByResult(MatchResult.Win);
        Debug.Log($"\n========== WINS ONLY ({wins.Length} matches) ==========");

        foreach (MatchData match in wins)
        {
            Debug.Log($"Match {match.matchId}: {match.playerScore}-{match.opponentScore} | {match.matchDate}");
        }


    }

    public void ShowLossesOnly()
    {
        MatchData[] losses = GetMatchesByResult(MatchResult.Lose);
        Debug.Log($"\n========== LOSSES ONLY ({losses.Length} matches) ==========");

        foreach (MatchData match in losses)
        {
            Debug.Log($"Match {match.matchId}: {match.playerScore}-{match.opponentScore} | {match.matchDate}");
        }


    }
}