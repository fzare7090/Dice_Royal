using RTLTMPro;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class RewardPanel : MonoBehaviour
{
    [Header("Reward UI")]
    [SerializeField] private GameObject coinItem;
    [SerializeField] private GameObject gemItem;
    [SerializeField] private GameObject cardItem;

    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text gemText;
    [SerializeField] private RTLTextMeshPro cardText;
    [SerializeField] private PlayerResourceData playerResource;
    [SerializeField] private Button collectButton;
    [SerializeField] private UIPanelAnimator animator;


    private RewardData rewardData;
    private RewardApplier rewardApplier = new RewardApplier();

    private void Awake()
    {
        animator = GetComponent<UIPanelAnimator>();
    }

    private void Start()
    {
        collectButton.onClick.AddListener(CollectReward);
    }

    public void ShowReward(RewardData reward)
    {
        if (reward == null)
        {
            Debug.LogError("Reward is NULL");
            return;
        }

        rewardData = reward;

        animator.Show();

        coinItem.SetActive(true);
        gemItem.SetActive(true);

        if (rewardData.cards != null && rewardData.cards.Count > 0)
            cardItem.SetActive(true);
        else
            cardItem.SetActive(false);

        animator.Show();
        StartCoroutine(SetupTextsDelayed());
    }

  
    private IEnumerator SetupTextsDelayed()
    {
        yield return null;

        coinText.text = rewardData.coins.ToString();
        gemText.text = rewardData.gems.ToString();

        if (rewardData.cards != null && rewardData.cards.Count > 0)
        {
            cardText.text =
                $"{rewardData.cards[0].cardId} x{rewardData.cards[0].amount}";
        }

        coinText.gameObject.SetActive(true);
        gemText.gameObject.SetActive(true);
        cardText.gameObject.SetActive(
            rewardData.cards != null &&
            rewardData.cards.Count > 0);
    }

    public void CollectReward()
    {
        rewardApplier.ApplyReward(
            rewardData,
            playerResource);

        SceneManager.LoadScene("GameHub");
    }
}