using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.InputSystem.InputControlScheme;

public class GameOverPanel : MonoBehaviour
{

    [SerializeField] private Chest chest;

    [SerializeField] private TMP_Text gameOverText;


    public Chest GetChest()
    {
        return chest;
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
}

