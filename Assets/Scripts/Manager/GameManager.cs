using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameTimeManager gameTime;
    [SerializeField] private Button buttonP1;
    [SerializeField] private Button buttonP2;
    [SerializeField] private GameOverPanel gameOverPanel;

    private Player player1;
    private Player player2;

    void Awake()
    {
        player1 = buttonP1.GetComponent<Player>();
        player2 = buttonP2.GetComponent<Player>();
    }

    void Start()
    {
        gameTime.StartTimer();
        StartCoroutine(CheckGameFlow());
    }

    private IEnumerator CheckGameFlow()
    {
        float normalPlayTime = 10f;
        float extraTime = 5f;
        float passedTime = 0f;

        while (passedTime < normalPlayTime)
        {
            passedTime += 1;


            int addingValueP1 = player1.GetElexir().GetAddingElexir();
            player1.GetElexir().GettingElexir(addingValueP1);


            int addingValueP2 = player2.GetElexir().GetAddingElexir();
            player2.GetElexir().GettingElexir(addingValueP2);


            if (player1.GetKingCastleConfig() != null)
            {
                if (!player1.GetKingCastleConfig().GetCastleHP().exist)
                {
                    gameTime.StopTimer();
                    Debug.Log("kingcastle player1 destroyed");

                    int winner = 1;
                    gameOverPanel.SetGameOverText(winner);

                    CheckForGameOver();
                    yield break;
                }
            }


            if (player2.GetKingCastleConfig() != null)
            {
                if (!player2.GetKingCastleConfig().GetCastleHP().exist)
                {
                    gameTime.StopTimer();
                    Debug.Log("kingcastle player2 destroyed");

                    int winner = 0;
                    gameOverPanel.SetGameOverText(winner);

                    CheckForGameOver();
                    yield break;
                }
            }

            yield return new WaitForSeconds(1);
        }


        if (player1.GetBoard() != null && player2.GetBoard() != null)
        {
            int princeCastleCount1 = 2;
            int princeCastleCount2 = 2;

            if (princeCastleCount1 != princeCastleCount2)
            {
                int winner;

                if (princeCastleCount1 > princeCastleCount2)
                {
                    winner = 0;
                }
                else
                {
                    winner = 1;
                }

                gameOverPanel.SetGameOverText(winner);
                gameTime.StopTimer();

                Debug.Log("normal time is over");

                CheckForGameOver();
                yield break;
            }
        }

        // Extra Time
        while (passedTime < extraTime + normalPlayTime)
        {
            passedTime += 1;

            int addingValueP1 = player1.GetElexir().GetExtraTimeElexirAdding();
            player1.GetElexir().GettingElexir(addingValueP1);

            int addingValueP2 = player2.GetElexir().GetExtraTimeElexirAdding();
            player2.GetElexir().GettingElexir(addingValueP2);

            if (player1.GetKingCastleConfig() != null)
            {
                if (!player1.GetKingCastleConfig().GetCastleHP().exist)
                {
                    gameTime.StopTimer();
                    Debug.Log("kingcastle player1 destroyed in extra time");

                    int winner = 1;
                    gameOverPanel.SetGameOverText(winner);

                    CheckForGameOver();
                    yield break;
                }
            }

            if (player2.GetKingCastleConfig() != null)
            {
                if (!player2.GetKingCastleConfig().GetCastleHP().exist)
                {
                    gameTime.StopTimer();
                    Debug.Log("kingcastle player2 destroyed in extra time");

                    int winner = 0;
                    gameOverPanel.SetGameOverText(winner);

                    CheckForGameOver();
                    yield break;
                }
            }

            yield return new WaitForSeconds(1);
        }


        int p1TotalHp = player1.GetKingCastleConfig().GetCastleHP().GetHPCastle();
        int p2TotalHp = player2.GetKingCastleConfig().GetCastleHP().GetHPCastle();

        if (p1TotalHp > p2TotalHp)
        {
            gameOverPanel.SetGameOverText(0);
        }
        else
        {
            gameOverPanel.SetGameOverText(1);
        }

        gameTime.StopTimer();
        Debug.Log("time is over");

        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (gameTime.GetRunningTimeBool() == false)
        {
            gameOverPanel.gameObject.SetActive(true);
        }
    }
}
