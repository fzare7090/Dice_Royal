using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameTimeManager gameTime;
    [SerializeField] private List<Player> players = new List<Player>();
    [SerializeField] private GameOverPanel gameOverPanel;

    private bool isGameFinished = false;
    private bool isEnding = false; 

    private bool isSuddenDeath = false;
    private bool isTieBreaker = false;

    private int suddenDeathStartCrowns0 = 0;
    private int suddenDeathStartCrowns1 = 0;

    private float lastInstantWinCheck = 0f;
    private float matchStartTime;

    private readonly Dictionary<int, List<Castle>> teamCastles = new Dictionary<int, List<Castle>>();

    void Start()
    {
        matchStartTime = Time.time;
    }

    private void Update()
    {
        if (isGameFinished || isEnding) return;

        if (Time.time - lastInstantWinCheck > 0.1f)
        {
            lastInstantWinCheck = Time.time;
            CheckInstantWinConditions();
        }
    }

    public void StartGameLogic()
    {
        Time.timeScale = 1f;
        CacheTeamCastles();
        gameTime.StartTimer();
        StartCoroutine(CheckGameFlow());
    }

    private IEnumerator CheckGameFlow()
    {
        while (!isGameFinished && !isEnding)
        {
            yield return new WaitForSeconds(1f);

            float currentGameTime = gameTime.GetCurrentTime();

            foreach (var player in players)
            {
                if (player == null) continue;
                if (player.GetKingCastleConfig() == null) continue;

                int addingValue =
                    currentGameTime < 60f ?
                    player.GetElexir().GetAddingElexir() :
                    player.GetElexir().GetExtraTimeElexirAdding();

                player.GetElexir().GettingElexir(addingValue);
            }

            if (CheckInstantWinConditions())
                yield break;

            if (!gameTime.GetRunningTimeBool())
            {
                int crowns0 = GetCrowns(0);
                int crowns1 = GetCrowns(1);

                if (!isSuddenDeath)
                {
                    if (crowns0 != crowns1)
                    {
                        EndMatch(crowns0 > crowns1 ? 0 : 1);
                        yield break;
                    }

                    isSuddenDeath = true;
                    suddenDeathStartCrowns0 = crowns0;
                    suddenDeathStartCrowns1 = crowns1;

                    gameTime.StartSuddenDeath(60f);
                    continue;
                }

                if (!isTieBreaker)
                {
                    isTieBreaker = true;
                    StartCoroutine(TieBreakerRoutine());
                    yield break;
                }
            }
        }
    }

    private bool CheckInstantWinConditions()
    {
        if (isGameFinished || isEnding)
            return false;

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (player == null)
                continue;

            var king = player.GetKingCastleConfig();

            if (king == null)
                continue;

            if (!king.exist || king.GetHPCastle() <= 0)
            {
                int winner = (i == 0) ? 1 : 0;

                HardEndMatch(winner);
                return true;
            }
        }

        return false;
    }


    private IEnumerator TieBreakerRoutine()
    {
        ClearBattlefield();

        List<Castle> castles = FindObjectsOfType<Castle>()
            .Where(c => c != null && c.exist)
            .ToList();

        while (!isGameFinished && !isEnding)
        {
            yield return null;

            if (CheckInstantWinConditions())
                yield break;

            float dmg = 300f * Time.deltaTime;

            bool team0Dead = false;
            bool team1Dead = false;

            foreach (var c in castles.ToList())
            {
                if (c == null || !c.exist) continue;

                int loserTeam = c.GetTeamId();

                c.TakeDamage(dmg);

                if (!c.exist)
                {
                    if (players.Count >= 2)
                    {
                        if (loserTeam == players[0].GetTeamId())
                            team0Dead = true;

                        if (loserTeam == players[1].GetTeamId())
                            team1Dead = true;
                    }
                }
            }

            if (team0Dead && !team1Dead)
            {
                EndMatch(1);
                yield break;
            }

            if (team1Dead && !team0Dead)
            {
                EndMatch(0);
                yield break;
            }
        }
    }

   

    private void ClearBattlefield()
    {
        foreach (var u in FindObjectsOfType<Unit>())
            if (u != null) Destroy(u.gameObject);

        foreach (var c in FindObjectsOfType<Castle>())
            if (c != null && c.exist) c.ClearCombat();
    }

    private int GetCrowns(int playerIndex)
    {
        int opponentIndex = playerIndex == 0 ? 1 : 0;

        if (players.Count <= opponentIndex || players[opponentIndex] == null)
            return 0;

        int opponentTeam = players[opponentIndex].GetTeamId();
        var castles = GetTeamCastles(opponentTeam);

        if (castles == null) return 0;

        bool kingAlive = castles.Any(c => c is KingCastleConfig k && k.exist && k.GetHPCastle() > 0);

        if (!kingAlive) return 3;

        return castles.Count(c => c is PrinceCastleConfig p && (!p.exist || p.GetHPCastle() <= 0));
    }

    private void CacheTeamCastles()
    {
        teamCastles.Clear();

        foreach (var c in FindObjectsOfType<Castle>())
        {
            if (c == null) continue;

            int team = c.GetTeamId();

            if (!teamCastles.ContainsKey(team))
                teamCastles[team] = new List<Castle>();

            teamCastles[team].Add(c);
        }
    }

    private List<Castle> GetTeamCastles(int teamId)
    {
        teamCastles.TryGetValue(teamId, out var list);
        return list;
    }

    private void CheckForGameOver()
    {
        if (gameOverPanel == null) return;

        gameOverPanel.animator.Show();
   
    }

    private void OnGameEnd(int winnerIndex, int p1, int p2)
    {
        float duration = Time.time - matchStartTime;

        if (DataGameManager.Instance != null)
        {
            DataGameManager.Instance.SaveMatchFromGameEnd(
                winnerIndex,
                p1,
                p2,
                duration,
                players[0].GetLevel(),
                players[1].GetLevel(),
                winnerIndex == 0 ? 20 : -15

            );
        }
        Debug.Log("winnerIndex = " + winnerIndex);
    }

    public int GetPlayerTowerCount(int playerIndex)
    {
        if (players.Count > playerIndex &&
            players[playerIndex] != null &&
            players[playerIndex].GetBoard() != null)
        {
            return players[playerIndex].GetBoard().GetCastleCount();
        }

        return 0;
    }
    private void EndMatch(int winnerIndex)
    {
        if (isGameFinished || isEnding) return;

        isEnding = true;
        isGameFinished = true;

        StopCoroutine(CheckGameFlow());
        StopCoroutine(TieBreakerRoutine());

        int p1 = GetPlayerTowerCount(0);
        int p2 = GetPlayerTowerCount(1);

        OnGameEnd(winnerIndex, p1, p2);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetGameOverText(winnerIndex);
            bool playerWon = winnerIndex == 0;
            gameOverPanel.SetupReward(playerWon);

           
            gameOverPanel.Show();

            
            StartCoroutine(StopTimeAfterPanelShown());
        }

        if (gameTime != null)
            gameTime.StopTimer();
    }

    private void HardEndMatch(int winnerIndex)
    {
        if (isGameFinished || isEnding) return;

        isEnding = true;
        isGameFinished = true;

        StopCoroutine(CheckGameFlow());
        StopCoroutine(TieBreakerRoutine());

        if (gameOverPanel != null)
        {
            gameOverPanel.SetGameOverText(winnerIndex);
            bool playerWon = winnerIndex == 0;
            gameOverPanel.SetupReward(playerWon);
            gameOverPanel.Show();
            StartCoroutine(StopTimeAfterPanelShown());
        }

        gameTime.StopTimer();
    }

    private IEnumerator StopTimeAfterPanelShown()
    {
       
        yield return new WaitForSecondsRealtime(0.5f);

              Time.timeScale = 0f;
    }

}