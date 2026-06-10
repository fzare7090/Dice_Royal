using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchHistoryItem : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private RTLTextMeshPro resultText;
    [SerializeField] private RTLTextMeshPro scoreText;
    [SerializeField] private RTLTextMeshPro trophyText;
    [SerializeField] private RTLTextMeshPro dateText;
    [SerializeField] private RTLTextMeshPro durationText;

    [Header("Levels")]
    [SerializeField] private RTLTextMeshPro opponentLevelText;

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

        // امتیاز
        scoreText.text =
            $"{data.playerScore} - {data.opponentScore}";

        // تاریخ
        dateText.text = data.matchDate;

        // مدت زمان
        durationText.text = FormatDuration(data.matchDuration);

        // تغییر جام
        trophyText.text =
            $"{data.trophiesChanged:+#;-#;0}";

        // سطح حریف
        opponentLevelText.text =
            $"سطح حریف: {data.opponentLevel}";

        // نتیجه مسابقه
        switch (data.result)
        {
            case MatchResult.Win:
                resultText.text = "پیروزی";
                backgroundImage.color = winColor;
                break;

            case MatchResult.Lose:
                resultText.text = "شکست";
                backgroundImage.color = loseColor;
                break;

            case MatchResult.Draw:
                resultText.text = "مساوی";
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