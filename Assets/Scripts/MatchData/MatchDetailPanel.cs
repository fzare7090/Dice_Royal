using TMPro;
using UnityEngine;

public class MatchDetailsPanel : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text durationText;

    [Header("Trophies")]
    [SerializeField] private TMP_Text trophiesBeforeText;
    [SerializeField] private TMP_Text trophiesAfterText;
    [SerializeField] private TMP_Text trophiesChangedText;

    [Header("Levels")]
    [SerializeField] private TMP_Text playerLevelText;
    [SerializeField] private TMP_Text opponentLevelText;

    //[SerializeField] private UIPanelAnimator animator;

    public void Show(MatchData match)
    {
        //animator.Show();

        resultText.text =
            match.result.ToString();

        scoreText.text =
            $"{match.playerScore} - {match.opponentScore}";

        dateText.text =
            match.matchDate;

        durationText.text =
            FormatDuration(match.matchDuration);

        trophiesBeforeText.text =
            match.trophiesBefore.ToString();

        trophiesAfterText.text =
            match.trophiesAfter.ToString();

        trophiesChangedText.text =
            $"{match.trophiesChanged:+#;-#;0}";

        playerLevelText.text =
            $"Lv.{match.playerLevel}";

        opponentLevelText.text =
            $"Lv.{match.opponentLevel}";
    }

  

    private string FormatDuration(float seconds)
    {
        int min = Mathf.FloorToInt(seconds / 60);
        int sec = Mathf.FloorToInt(seconds % 60);

        return $"{min:00}:{sec:00}";
    }
}  

