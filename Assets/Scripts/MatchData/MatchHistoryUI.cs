using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchHistoryUI : MonoBehaviour
{
    [Header("Summary")]
    [SerializeField] private TMP_Text totalMatchesText;
    [SerializeField] private TMP_Text winsText;
    [SerializeField] private TMP_Text lossesText;
    [SerializeField] private TMP_Text winRateText;

    [Header("Content")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private MatchHistoryItem itemPrefab;

    private readonly List<MatchHistoryItem> activeItems =
        new List<MatchHistoryItem>();


    private void Start()
    {
        Debug.Log("history ");
    }
    private void OnEnable()
    {
        Refresh();
        Debug.Log("MatchHistoryUI Started");
    }

    public void Refresh()
    {
        UpdateSummary();
        LoadAllMatches();
    }

    public void LoadAllMatches()
    {
        Populate(
            DataGameManager.Instance.GetAllMatches());
    }

    public void LoadWins()
    {
        Populate(
            DataGameManager.Instance.GetMatchesByResult(
                MatchResult.Win));
    }

    public void LoadLosses()
    {
        Populate(
            DataGameManager.Instance.GetMatchesByResult(
                MatchResult.Lose));
    }

    public void LoadDraws()
    {
        Populate(
            DataGameManager.Instance.GetMatchesByResult(
                MatchResult.Draw));
    }

    private void Populate(MatchData[] matches)
    {
        ClearItems();

        for (int i = matches.Length - 1; i >= 0; i--)
        {
            if (matches[i] == null)
                continue;
            Debug.Log("Creating Item");
            MatchHistoryItem item =
                Instantiate(itemPrefab, contentParent);

            item.Setup(matches[i]);

            activeItems.Add(item);
        }


        Debug.Log("Matches Found: " + matches.Length);
    }

    private void ClearItems()
    {
        foreach (var item in activeItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        activeItems.Clear();
    }
    private void UpdateSummary()
    {
        Debug.Log("DataManager = " + DataGameManager.Instance);

        //Debug.Log("totalMatchesText = " + totalMatchesText);
        //Debug.Log("winsText = " + winsText);
        //Debug.Log("lossesText = " + lossesText);
        //Debug.Log("winRateText = " + winRateText);

        if (DataGameManager.Instance == null)
        {
            Debug.LogError("DataGameManager NULL");
            return;
        }

        GameStatsData stats =
            DataGameManager.Instance.GetGameStats();

        if (totalMatchesText != null)
            totalMatchesText.text = stats.totalMatches.ToString();

        if (winsText != null)
            winsText.text = stats.totalWins.ToString();

        if (lossesText != null)
            lossesText.text = stats.totalLosses.ToString();

        if (winRateText != null)
            winRateText.text = $"{stats.winRate:0}%";
    }
}