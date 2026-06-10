using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameTimeManager gameTime;
    [SerializeField] private List<Player> players = new List<Player>();
    [SerializeField] private GameOverPanel gameOverPanel;
    [SerializeField] private List<UnitData> playerDeck = new List<UnitData>();

    private bool isGameFinished = false;
    private bool isSuddenDeath = false;
    private bool isTieBreaker = false;

    private int suddenDeathStartCrowns0 = 0;
    private int suddenDeathStartCrowns1 = 0;
    private float lastInstantWinCheck = 0f;
    private float matchStartTime; // اضافه شده از کد دوستت برای محاسبه مدت زمان بازی

    private readonly Dictionary<int, List<Castle>> teamCastles = new Dictionary<int, List<Castle>>();

    void Start()
    {
        // آغاز زمان‌سنجی واقعی مسابقه برای بخش دیتا
        matchStartTime = Time.time;
    }

    private void Update()
    {
        if (isGameFinished) return;

        if (Time.time - lastInstantWinCheck > 0.1f)
        {
            lastInstantWinCheck = Time.time;
            CheckInstantWinConditions();
        }
    }

    public void StartGameLogic()
    {
        Time.timeScale = 1f;
        matchStartTime = Time.time;
        CacheTeamCastles();
        gameTime.StartTimer();
        StartCoroutine(CheckGameFlow());
    }

    // متد ثبت اطلاعات در دیتابیس (آورده شده از کد دوستت و اصلاح شده)
    private void OnGameEnd(int winnerIndex, int player1Towers, int player2Towers)
    {
        float matchDuration = gameTime != null ? gameTime.GetCurrentTime() : Time.time - matchStartTime;

        if (players.Count >= 2 && players[0] != null && players[1] != null)
        {
            int playerLevel = players[0].GetLevel();
            int opponentLevel = players[1].GetLevel();

            if (DataGameManager.Instance != null)
            {
                DataGameManager.Instance.SaveMatchFromGameEnd(
                    winnerIndex: winnerIndex,
                    player1Score: player1Towers,
                    player2Score: player2Towers,
                    duration: matchDuration,
                    playerLevel: playerLevel,
                    opponentLevel: opponentLevel,
                    trophiesChange: winnerIndex == 0 ? 20 : -15
                );
            }
        }
    }

    private IEnumerator CheckGameFlow()
    {
        while (!isGameFinished)
        {
            yield return new WaitForSeconds(1f);

            float currentGameTime = gameTime.GetCurrentTime();
            foreach (var player in players)
            {
                if (player == null) continue;
                int addingValue = currentGameTime < 60f ?
                    player.GetElexir().GetAddingElexir() :
                    player.GetElexir().GetExtraTimeElexirAdding();
                player.GetElexir().GettingElexir(addingValue);
            }

            if (CheckInstantWinConditions())
            {
                yield break;
            }

            if (!gameTime.GetRunningTimeBool())
            {
                if (players.Count >= 2)
                {
                    int crowns0 = GetCrowns(0);
                    int crowns1 = GetCrowns(1);

                    if (!isSuddenDeath)
                    {
                        if (crowns0 != crowns1)
                        {
                            int winnerIndex = crowns0 > crowns1 ? 0 : 1;
                            EndMatch(winnerIndex);
                            yield break;
                        }
                        else
                        {
                            isSuddenDeath = true;
                            suddenDeathStartCrowns0 = crowns0;
                            suddenDeathStartCrowns1 = crowns1;
                            gameTime.StartSuddenDeath(60f);

                            yield return null;
                            continue;
                        }
                    }
                    else if (!isTieBreaker)
                    {
                        isTieBreaker = true;
                        StartCoroutine(TieBreakerRoutine());
                        yield break;
                    }
                }
                else
                {
                    EndMatch(0);
                    yield break;
                }
            }
        }
    }

    private int GetCrowns(int playerIndex)
    {
        int crowns = 0;
        int opponentIndex = playerIndex == 0 ? 1 : 0;

        if (players.Count <= opponentIndex || players[opponentIndex] == null) return 0;

        int opponentTeamId = players[opponentIndex].GetTeamId();
        var opponentCastles = GetTeamCastles(opponentTeamId);
        if (opponentCastles == null || opponentCastles.Count == 0) return 0;

        bool kingAlive = false;
        foreach (var c in opponentCastles)
        {
            if (c == null) continue;
            if (c is KingCastleConfig)
            {
                kingAlive = c.exist && c.GetHPCastle() > 0f;
                break;
            }
        }

        if (!kingAlive)
        {
            return 3;
        }

        foreach (var c in opponentCastles)
        {
            if (c is PrinceCastleConfig)
            {
                if (c == null || !c.exist || c.GetHPCastle() <= 0f)
                {
                    crowns++;
                }
            }
        }

        return crowns;
    }

    private void CacheTeamCastles()
    {
        teamCastles.Clear();
        foreach (var castle in FindObjectsOfType<Castle>())
        {
            if (castle == null) continue;
            int teamId = castle.GetTeamId();
            if (!teamCastles.ContainsKey(teamId))
            {
                teamCastles[teamId] = new List<Castle>();
            }
            teamCastles[teamId].Add(castle);
        }
    }

    private List<Castle> GetTeamCastles(int teamId)
    {
        if (teamCastles.TryGetValue(teamId, out var list))
        {
            return list;
        }
        return null;
    }

    private IEnumerator TieBreakerRoutine()
    {
        ClearBattlefield();

        List<Castle> tiebreakerCastles = new List<Castle>();
        foreach (var c in FindObjectsOfType<Castle>())
        {
            if (c != null && c.exist) tiebreakerCastles.Add(c);
        }

        while (!isGameFinished)
        {
            if (CheckInstantWinConditions())
            {
                yield break;
            }

            float damageToDeal = 300f * Time.deltaTime;

            bool team0DiedThisFrame = false;
            bool team1DiedThisFrame = false;

            foreach (var c in tiebreakerCastles)
            {
                if (c != null && c.exist)
                {
                    c.TakeDamage(damageToDeal);

                    if (!c.exist || c.GetHPCastle() <= 0f)
                    {
                        int loserTeam = c.GetTeamId();

                        if (players.Count >= 2)
                        {
                            if (loserTeam == players[0].GetTeamId()) team0DiedThisFrame = true;
                            else if (loserTeam == players[1].GetTeamId()) team1DiedThisFrame = true;
                        }

                        if (c is KingCastleConfig)
                        {
                            foreach (var other in tiebreakerCastles)
                            {
                                if (other != null && other.exist && other.GetTeamId() == loserTeam && other is PrinceCastleConfig)
                                {
                                    other.TakeDamage(99999f);
                                }
                            }
                        }
                    }
                }
            }

            if (team0DiedThisFrame && !team1DiedThisFrame)
            {
                EndMatch(1);
                yield break;
            }
            else if (team1DiedThisFrame && !team0DiedThisFrame)
            {
                EndMatch(0);
                yield break;
            }

            yield return null;
        }
    }

    private void ClearBattlefield()
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        foreach (var u in allUnits)
        {
            if (u != null) Destroy(u.gameObject);
        }

        Castle[] allCastles = FindObjectsOfType<Castle>();
        foreach (var c in allCastles)
        {
            if (c != null && c.exist)
            {
                c.ClearCombat();
            }
        }

        Spawner[] spawners = FindObjectsOfType<Spawner>();
        foreach (var s in spawners)
        {
            if (s != null) s.enabled = false;
        }

        UnitCardDrag[] draggers = FindObjectsOfType<UnitCardDrag>();
        foreach (var d in draggers)
        {
            if (d != null) d.enabled = false;
        }

        CardButtonUI[] buttons = FindObjectsOfType<CardButtonUI>();
        foreach (var b in buttons)
        {
            if (b != null && b.GetButton() != null)
                b.GetButton().interactable = false;
        }
    }

    private void EndMatch(int winnerIndex)
    {
        if (isGameFinished) return;

        isGameFinished = true;

        // Crowns are the destroyed opponent castles, not the remaining castles.
        int p1DestroyedCastles = GetCrowns(0);
        int p2DestroyedCastles = GetCrowns(1);

        // ذخیره اطلاعات مچ در دیتابیس بازی
        OnGameEnd(winnerIndex, p1DestroyedCastles, p2DestroyedCastles);

        // اعمال یوآی پایان بازی
        if (gameOverPanel != null)
        {
            gameOverPanel.SetGameOverText(winnerIndex);
            gameOverPanel.GivingChest(winnerIndex);
        }

        if (gameTime != null) gameTime.StopTimer();

        CheckForGameOver();
    }

    private bool CheckInstantWinConditions()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null) continue;

            var kingCastle = players[i].GetKingCastleConfig();
            if (kingCastle == null || !kingCastle.exist || kingCastle.GetHPCastle() <= 0f)
            {
                var board = players[i].GetBoard();
                if (board != null)
                {
                    var rightPrince = board.GetRightPrinceCastles();
                    if (rightPrince != null && rightPrince.exist)
                        rightPrince.TakeDamage(rightPrince.GetHPCastle() + 100f);

                    var leftPrince = board.GetLeftPrinceCastles();
                    if (leftPrince != null && leftPrince.exist)
                        leftPrince.TakeDamage(leftPrince.GetHPCastle() + 100f);
                }

                int winner = (i == 0) ? 1 : 0;
                EndMatch(winner);
                return true;
            }
        }

        if (isSuddenDeath && players.Count >= 2)
        {
            int currentCrowns0 = GetCrowns(0);
            int currentCrowns1 = GetCrowns(1);

            if (currentCrowns0 > suddenDeathStartCrowns0)
            {
                EndMatch(0);
                return true;
            }
            else if (currentCrowns1 > suddenDeathStartCrowns1)
            {
                EndMatch(1);
                return true;
            }
        }

        return false;
    }

    private void CheckForGameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null)
        {
            gameOverPanel.gameObject.SetActive(true);
            gameOverPanel.ShowAnimated();
        }
    }

    // این متد کدهای تو بود که دوستت حذفش کرده بود و دوباره اضافه‌اش کردم
    public int GetPlayerTowerCount(int playerIndex)
    {
        if (players.Count > playerIndex && players[playerIndex] != null && players[playerIndex].GetBoard() != null)
        {
            return players[playerIndex].GetBoard().GetCastleCount();
        }
        return 0;
    }
}
