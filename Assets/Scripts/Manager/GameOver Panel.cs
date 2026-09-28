using TMPro;
using UnityEngine;
using RTLTMPro;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class GameOverPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RTLTextMeshPro resultText;
    [SerializeField] private RTLTextMeshPro subTitleText;

    [SerializeField] private RectTransform panelContainer;
    [SerializeField] private CanvasGroup canvasGroup;


    [SerializeField] private RewardPanel rewardPanel;
    [SerializeField] private ChestRewardGenerator rewardGenerator;
    [SerializeField] private ChestData chestData;


    [SerializeField] private Button openRewardPanel;
    public UIPanelAnimator animator;
    private RewardData currentReward;

    private void Awake()
    {
        animator = GetComponent<UIPanelAnimator>();
        canvasGroup = GetComponent<CanvasGroup>();
    }


    private void Start()
    {
        openRewardPanel.onClick.AddListener(OpenRewardPanel);
    }

    public void SetGameOverText(int winnerIndex)
    {
        bool playerWon = winnerIndex == 0;

        if (playerWon)
        {
            resultText.text = "پیروزی";
            subTitleText.text = "تبریک! مسابقه را بردی";
        }
        else
        {
            resultText.text = "شکست";
            subTitleText.text = "متاسفانه مسابقه را باختی";
        }
    }


    public void Hide()
    {
        animator.Hide();
        
    }
    public void OpenRewardPanel()
    {
        animator.Hide();
        rewardPanel.ShowReward(currentReward);
    }

    public void SetupReward(bool isWinner)
    {
        MatchResult result =
            isWinner
            ? MatchResult.Win
            : MatchResult.Lose;

        currentReward =
            rewardGenerator.GenerateReward(
                chestData,
                result);
    }

    public void Show()
    {
        Debug.Log("GameOverPanel Show");
        animator.Show();
    }
}