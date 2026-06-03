using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Chest chest;
    [SerializeField] private TMP_Text gameOverText;

    private float matchStartTime;

    void Start()
    {
        matchStartTime = Time.time;
    }

    public void SetGameOverText(int winnerIndex)
    {
        gameOverText.SetText($"Winner is : \n Player {winnerIndex + 1}");

    }

    public void GivingChest(int winnerIndex)
    {
        chest.GiveMatchRewards(winnerIndex);
        chest.SetRewardsText(winnerIndex);
    }

    private int GetPlayerTowers(int playerIndex)
    {
        // پیدا کردن GameManager و گرفتن امتیاز
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            // اینجا باید یه تابع عمومی در GameManager بسازی که امتیاز رو برگردونه
            return gm.GetPlayerTowerCount(playerIndex);
        }
        return 0;
    }
}