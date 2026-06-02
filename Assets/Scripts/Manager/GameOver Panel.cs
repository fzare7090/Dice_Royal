using TMPro;
using UnityEngine;
using RTLTMPro;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.InputSystem.InputControlScheme;

public class GameOverPanel : MonoBehaviour
{

    [SerializeField] private Chest chest;

    [SerializeField] private RTLTextMeshPro gameOverText;


    public Chest GetChest()
    {
        return chest;
    }
    public void SetGameOverText(int winnerIndex)
    {
        if (winnerIndex == -1)
        {
            gameOverText.text = " بازی مساوی شد! ";
        }
        else
        {
            gameOverText.text = $" برنده بازی\nبازیکن {winnerIndex + 1}";
        }
    }

    public void GivingChest(int winnerIndex)
    {
        if (winnerIndex == -1) return; 

        chest.GiveMatchRewards(winnerIndex);
        chest.SetRewardsText(winnerIndex);
    }
}

