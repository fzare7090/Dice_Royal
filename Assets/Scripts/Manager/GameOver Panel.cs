using TMPro;
using UnityEngine;

using RTLTMPro;
using DG.Tweening;


[RequireComponent(typeof(CanvasGroup))]
public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Chest chest;

    [SerializeField] private RTLTextMeshPro gameOverText;


    [SerializeField] private RectTransform panelContainer;
    [SerializeField] private CanvasGroup canvasGroup;




    private float matchStartTime;


    void Start()
    {
        matchStartTime = Time.time;
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



    public void ShowAnimated()
    {
        gameObject.SetActive(true);


        canvasGroup.alpha = 0f;
        panelContainer.localScale = Vector3.zero;
        gameOverText.transform.localScale = Vector3.zero;

        if (chest != null)
            chest.transform.localScale = Vector3.zero;


        Sequence seq = DOTween.Sequence();


        seq.SetUpdate(true);


        seq.Append(canvasGroup.DOFade(1f, 0.3f));


        seq.Append(panelContainer.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));


        seq.Insert(0.5f, gameOverText.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBounce));


        if (chest != null)
        {
            seq.Insert(0.7f, chest.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutElastic));
        }
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