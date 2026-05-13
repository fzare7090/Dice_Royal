using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{

    [SerializeField] private TMP_Text gameOverText;


    public void SetGameOverText(int winnerIndex)
    {
        gameOverText.SetText($"Winner is : \n Player {winnerIndex+1}");
    }
}

