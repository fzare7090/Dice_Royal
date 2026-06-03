using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchHistoryItem : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text trophyText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text durationText;

    [Header("Levels")]
    //[SerializeField] private TMP_Text playerLevelText;
    [SerializeField] private TMP_Text opponentLevelText;

    [Header("UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button detailsButton;


    [Header("Colors")]
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private Color drawColor;

    private MatchData currentMatch;

    public void Setup(MatchData data)
    {
        currentMatch = data;

        scoreText.text =
            $"{data.playerScore} - {data.opponentScore}";

        dateText.text = data.matchDate;

        durationText.text = FormatDuration(data.matchDuration);

        trophyText.text =
            $"{data.trophiesChanged:+#;-#;0} 🏆";

        opponentLevelText.text =
            $"ENEMY Lv.{data.opponentLevel}";

        switch (data.result)
        {
            case MatchResult.Win:
                resultText.text = "VICTORY";
                backgroundImage.color = winColor;
                break;

            case MatchResult.Lose:
                resultText.text = "DEFEAT";
                backgroundImage.color = loseColor;
                break;

            case MatchResult.Draw:
                resultText.text = "DRAW";
                backgroundImage.color = drawColor;
                break;
        }

        if (detailsButton != null)
        {
            detailsButton.onClick.RemoveAllListeners();
            detailsButton.onClick.AddListener(OpenDetails);
        }
    }

    private void OpenDetails()
    {
        MatchDetailsPanel detailsPanel =
            FindObjectOfType<MatchDetailsPanel>(true);

        if (detailsPanel != null)
        {
            detailsPanel.Show(currentMatch);
        }
    }

    private string FormatDuration(float seconds)
    {
        int min = Mathf.FloorToInt(seconds / 60);
        int sec = Mathf.FloorToInt(seconds % 60);

        return $"{min:00}:{sec:00}";
    }
}